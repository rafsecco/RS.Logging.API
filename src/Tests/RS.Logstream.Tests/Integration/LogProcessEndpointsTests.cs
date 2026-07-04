using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RS.Logstream.API.ViewModels;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Tests.Integration.Helpers;
using Xunit;

namespace RS.Logstream.Tests.Integration;

public class LogProcessEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;

	public LogProcessEndpointsTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	private void Authenticate(string tenantId)
	{
		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTestTokenFactory.CreateValidToken());
		_client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);
	}

	private async Task SeedProcessesAsync(params LogProcess[] processes)
	{
		using var scope = _factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<RSLogstreamDbContext>();
		context.LogProcess.AddRange(processes);
		await context.SaveChangesAsync();
	}

	#region Error

	[Fact]
	public async Task Post_WithoutToken_ReturnsUnauthorized()
	{
		// Arrange
		var body = new LogProcessViewModel(1, "Process without token");

		// Act
		var response = await _client.PostAsJsonAsync("/log-process", body);

		// Assert
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	#endregion

	#region Success

	[Fact]
	public async Task Post_ValidProcess_ReturnsOkWithGeneratedId()
	{
		// Arrange
		Authenticate(Guid.NewGuid().ToString());
		var body = new LogProcessViewModel(1001, "Pedido #1001 — checkout");

		// Act
		var response = await _client.PostAsJsonAsync("/log-process", body);
		var id = await response.Content.ReadFromJsonAsync<long>();

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.True(id > 0);
	}

	[Fact]
	public async Task PostDetail_ValidDetail_ReturnsAcceptedAndPersists()
	{
		// Arrange
		Authenticate(Guid.NewGuid().ToString());
		var createResponse = await _client.PostAsJsonAsync("/log-process", new LogProcessViewModel(1002, "Pedido #1002"));
		var processId = await createResponse.Content.ReadFromJsonAsync<long>();
		var message = $"detail-{Guid.NewGuid()}";
		var detailBody = new LogProcessDetailsViewModel(processId, LogLevel.Information, message, null);

		// Act
		var response = await _client.PostAsJsonAsync("/log-process/detail", detailBody);
		var process = await PollingHelper.WaitUntilAsync(
			async () => await _client.GetFromJsonAsync<LogProcessDto>($"/log-process/{processId}", TestJson.Options),
			p => p?.LorProcessDetailList?.Any(d => d.Message == message) == true);

		// Assert
		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
		Assert.Contains(process.LorProcessDetailList!, d => d.Message == message);
	}

	[Fact]
	public async Task GetAll_WithPageSizeOne_ReturnsOnePage()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		await SeedProcessesAsync(
			new LogProcess(1, "Process A", pTenantId: tenantId),
			new LogProcess(2, "Process B", pTenantId: tenantId));

		// Act
		var processes = await _client.GetFromJsonAsync<List<LogProcessDto>>("/log-process?pn=1&ps=1", TestJson.Options);

		// Assert
		Assert.NotNull(processes);
		Assert.Single(processes);
	}

	[Fact]
	public async Task Search_ByProcessName_ReturnsOnlyMatchingProcess()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		var matching = new LogProcess(1, "Payment checkout", pTenantId: tenantId);
		var other = new LogProcess(2, "Inventory reservation", pTenantId: tenantId);
		await SeedProcessesAsync(matching, other);
		await SeedDetailAsync(matching.Id, "Etapa 1");
		await SeedDetailAsync(other.Id, "Etapa 1");

		// Act
		var results = await _client.GetFromJsonAsync<List<LogProcessDto>>("/log-process/search?nm=Payment", TestJson.Options);

		// Assert
		Assert.NotNull(results);
		Assert.Single(results);
		Assert.Equal("Payment checkout", results[0].Name);
	}

	private async Task SeedDetailAsync(long logProcessId, string message)
	{
		using var scope = _factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<RSLogstreamDbContext>();
		context.LogProcessDetails.Add(new LogProcessDetail(logProcessId, LogLevel.Information, message));
		await context.SaveChangesAsync();
	}

	#endregion
}
