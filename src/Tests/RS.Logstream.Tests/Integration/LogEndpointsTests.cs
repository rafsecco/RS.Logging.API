using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RS.Logstream.API.ViewModels;
using RS.Logstream.Domain.Log;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Tests.Integration.Helpers;
using Xunit;

namespace RS.Logstream.Tests.Integration;

public class LogEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;

	public LogEndpointsTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	private void Authenticate(string tenantId)
	{
		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTestTokenFactory.CreateValidToken());
		_client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);
	}

	private async Task SeedLogsAsync(params Log[] logs)
	{
		using var scope = _factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<RSLogstreamDbContext>();
		context.Logs.AddRange(logs);
		await context.SaveChangesAsync();
	}

	#region Error

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized()
	{
		// Act
		var response = await _client.GetAsync("/logs");

		// Assert
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	#endregion

	#region Success

	[Fact]
	public async Task Post_ValidLog_ReturnsAccepted()
	{
		// Arrange
		Authenticate(Guid.NewGuid().ToString());
		var body = new LogsViewModel(LogLevel.Information, $"log-{Guid.NewGuid()}", null);

		// Act
		var response = await _client.PostAsJsonAsync("/logs", body);

		// Assert
		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
	}

	[Fact]
	public async Task Post_ValidLog_PersistsAndIsRetrievableViaGet()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		var message = $"log-{Guid.NewGuid()}";
		var body = new LogsViewModel(LogLevel.Warning, message, null);

		// Act
		await _client.PostAsJsonAsync("/logs", body);
		var logs = await PollingHelper.WaitUntilAsync(
			async () => await _client.GetFromJsonAsync<List<LogDto>>("/logs", TestJson.Options),
			list => list is not null && list.Any(l => l.Message == message));

		// Assert
		var found = Assert.Single(logs, l => l.Message == message);
		Assert.Equal(LogLevel.Warning, found.LogLevel);
	}

	[Fact]
	public async Task GetAll_WithPageSizeTwo_ReturnsOnlyTwoItems()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		await SeedLogsAsync(
			new Log(LogLevel.Information, "Log 1", pTenantId: tenantId),
			new Log(LogLevel.Information, "Log 2", pTenantId: tenantId),
			new Log(LogLevel.Information, "Log 3", pTenantId: tenantId));

		// Act
		var logs = await _client.GetFromJsonAsync<List<LogDto>>("/logs?pn=1&ps=2", TestJson.Options);

		// Assert
		Assert.NotNull(logs);
		Assert.Equal(2, logs.Count);
	}

	[Fact]
	public async Task Search_ByLogLevel_ReturnsOnlyMatchingLogs()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		await SeedLogsAsync(
			new Log(LogLevel.Information, "Info log", pTenantId: tenantId),
			new Log(LogLevel.Error, "Error log 1", pTenantId: tenantId),
			new Log(LogLevel.Error, "Error log 2", pTenantId: tenantId));

		// Act
		var logs = await _client.GetFromJsonAsync<List<LogDto>>($"/logs/search?ll={(int)LogLevel.Error}", TestJson.Options);

		// Assert
		Assert.NotNull(logs);
		Assert.Equal(2, logs.Count);
		Assert.All(logs, l => Assert.Equal(LogLevel.Error, l.LogLevel));
	}

	#endregion
}
