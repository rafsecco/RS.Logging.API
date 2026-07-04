using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RS.Logstream.API.ViewModels;
using RS.Logstream.Domain.ApiCall;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Tests.Integration.Helpers;
using Xunit;

namespace RS.Logstream.Tests.Integration;

public class ApiCallLogEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;

	public ApiCallLogEndpointsTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	private void Authenticate(string tenantId)
	{
		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtTestTokenFactory.CreateValidToken());
		_client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);
	}

	private async Task SeedEntriesAsync(params ApiCallLog[] entries)
	{
		using var scope = _factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<RSLogstreamDbContext>();
		context.ApiCallLogs.AddRange(entries);
		await context.SaveChangesAsync();
	}

	#region Error

	[Fact]
	public async Task Post_WithoutToken_ReturnsUnauthorized()
	{
		// Arrange
		var body = new ApiCallLogViewModel("https://api.exemplo.com/v1", "GET", true, null, null, 200, null, 100, null);

		// Act
		var response = await _client.PostAsJsonAsync("/api-call-log", body);

		// Assert
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	#endregion

	#region Success

	[Fact]
	public async Task Post_ValidEntry_ReturnsAcceptedAndPersists()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		var url = $"https://api.exemplo.com/v1/{Guid.NewGuid()}";
		var body = new ApiCallLogViewModel(url, "POST", true, "{}", null, 200, "{}", 120, null);

		// Act
		var response = await _client.PostAsJsonAsync("/api-call-log", body);
		var entries = await PollingHelper.WaitUntilAsync(
			async () => await _client.GetFromJsonAsync<List<ApiCallLogDto>>("/api-call-log", TestJson.Options),
			list => list is not null && list.Any(e => e.Url == url));

		// Assert
		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
		Assert.Contains(entries, e => e.Url == url);
	}

	[Fact]
	public async Task GetAll_WithPageSizeOne_ReturnsOnePage()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		await SeedEntriesAsync(
			new ApiCallLog("https://api.exemplo.com/a", "GET", true, pTenantId: tenantId),
			new ApiCallLog("https://api.exemplo.com/b", "GET", true, pTenantId: tenantId));

		// Act
		var entries = await _client.GetFromJsonAsync<List<ApiCallLogDto>>("/api-call-log?pn=1&ps=1", TestJson.Options);

		// Assert
		Assert.NotNull(entries);
		Assert.Single(entries);
	}

	[Fact]
	public async Task Search_ByStatusCode_ReturnsOnlyMatchingEntries()
	{
		// Arrange
		var tenantId = Guid.NewGuid().ToString();
		Authenticate(tenantId);
		await SeedEntriesAsync(
			new ApiCallLog("https://api.exemplo.com/ok", "GET", true, pResponseStatusCode: 200, pTenantId: tenantId),
			new ApiCallLog("https://api.exemplo.com/erro", "GET", false, pResponseStatusCode: 500, pTenantId: tenantId));

		// Act
		var entries = await _client.GetFromJsonAsync<List<ApiCallLogDto>>("/api-call-log/search?sc=500", TestJson.Options);

		// Assert
		Assert.NotNull(entries);
		var found = Assert.Single(entries);
		Assert.Equal(500, found.ResponseStatusCode);
	}

	#endregion
}
