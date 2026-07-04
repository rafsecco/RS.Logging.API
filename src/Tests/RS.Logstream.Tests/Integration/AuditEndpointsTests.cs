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

public class AuditEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;

	public AuditEndpointsTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	private void Authenticate(string tenantId)
	{
		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTestTokenFactory.CreateValidToken());
		_client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);
	}

	private async Task<LogProcess> SeedProcessAsync(string tenantId, string name, params LogProcessDetail[] details)
	{
		using var scope = _factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<RSLogstreamDbContext>();
		var process = new LogProcess(1, name, pTenantId: tenantId);
		context.LogProcess.Add(process);
		await context.SaveChangesAsync();

		foreach (var detail in details)
		{
			detail.LogProcessId = process.Id;
			context.LogProcessDetails.Add(detail);
		}
		await context.SaveChangesAsync();

		return process;
	}

	#region Error

	[Fact]
	public async Task GetById_WithoutToken_ReturnsUnauthorized()
	{
		// Act
		var response = await _client.GetAsync("/log-process/audit/1");

		// Assert
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetById_NonExistingId_ReturnsNotFound()
	{
		// Arrange
		Authenticate(Guid.NewGuid().ToString());

		// Act
		var response = await _client.GetAsync("/log-process/audit/999999999");

		// Assert
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	#endregion

	#region Success

	[Fact]
	public async Task GetById_ExistingProcess_ReturnsAuditViewModel()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		var process = await SeedProcessAsync(tenantId, "Pedido auditado",
			new LogProcessDetail(0, LogLevel.Information, "Etapa 1"));

		// Act
		var audit = await _client.GetFromJsonAsync<AuditLogProcessViewModel>($"/log-process/audit/{process.Id}", TestJson.Options);

		// Assert
		Assert.NotNull(audit);
		Assert.Equal(process.Id, audit.Id);
		Assert.Equal(ProcessStatus.Success, audit.Status);
	}

	[Fact]
	public async Task GetAll_WithStatusFilter_ReturnsOnlyMatchingStatus()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		await SeedProcessAsync(tenantId, "Processo ok",
			new LogProcessDetail(0, LogLevel.Information, "Etapa 1"));
		var warningProcess = await SeedProcessAsync(tenantId, "Processo com aviso",
			new LogProcessDetail(0, LogLevel.Warning, "Etapa com problema"));

		// Act
		var results = await _client.GetFromJsonAsync<List<AuditLogProcessViewModel>>(
			$"/log-process/audit?st={(int)ProcessStatus.Warning}", TestJson.Options);

		// Assert
		Assert.NotNull(results);
		var found = Assert.Single(results);
		Assert.Equal(warningProcess.Id, found.Id);
		Assert.Equal(ProcessStatus.Warning, found.Status);
	}

	#endregion
}
