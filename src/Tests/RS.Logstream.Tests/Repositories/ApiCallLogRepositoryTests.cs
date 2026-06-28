using Microsoft.EntityFrameworkCore;
using RS.Logstream.Domain.ApiCall;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Providers;
using RS.Logstream.Infra.Repositories;
using Xunit;

namespace RS.Logstream.Tests.Repositories;

public class ApiCallLogRepositoryTests
{
	private static RSLogstreamDbContext CreateContext() =>
		new(new DbContextOptionsBuilder<RSLogstreamDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options, new MariaDbColumnTypes());

	private static ApiCallLogRepository Repo(RSLogstreamDbContext ctx) =>
		new(ctx);

	private static ApiCallLog Success(string url = "https://api.pagamentos.com/charge", string? tenantId = null, string? correlationId = null, string? traceId = null) =>
		new(url, "POST", true, pResponseStatusCode: 200, pDurationMs: 120, pTenantId: tenantId, pCorrelationId: correlationId, pTraceId: traceId);

	private static ApiCallLog Failure(string url = "https://api.estoque.com/reservar", int? statusCode = 503, string? tenantId = null, string? correlationId = null, string? traceId = null) =>
		new(url, "POST", false, pResponseStatusCode: statusCode, pErrorMessage: "Service unavailable", pTenantId: tenantId, pCorrelationId: correlationId, pTraceId: traceId);

	#region Success

	[Fact]
	public void Create_ValidApiCallLog_ReturnsTrue()
	{
		using var context = CreateContext();
		var result = Repo(context).Create(Success());
		Assert.True(result);
	}

	[Fact]
	public void Create_ValidApiCallLog_PersistsToDatabase()
	{
		using var context = CreateContext();
		var entry = Success();
		Repo(context).Create(entry);
		Assert.Single(context.ApiCallLogs);
	}

	[Fact]
	public void GetById_ExistingId_ReturnsApiCallLog()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		var entry = Success();
		repo.Create(entry);

		var result = repo.GetById(entry.Id);

		Assert.NotNull(result);
		Assert.Equal("https://api.pagamentos.com/charge", result.Url);
		Assert.True(result.IsSuccess);
	}

	[Fact]
	public void GetById_FilterByTenantId_ReturnsEntry()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		var entry = Success(tenantId: "appA");
		repo.Create(entry);

		var result = repo.GetById(entry.Id, tenantId: "appA");

		Assert.NotNull(result);
	}

	[Fact]
	public void GetById_DifferentTenant_ReturnsNull()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		var entry = Success(tenantId: "appA");
		repo.Create(entry);

		var result = repo.GetById(entry.Id, tenantId: "appB");

		Assert.Null(result);
	}

	[Fact]
	public void GetAll_WithEntries_ReturnsList()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success());
		repo.Create(Failure());

		var result = repo.GetAll(null, null).ToList();

		Assert.Equal(2, result.Count);
	}

	[Fact]
	public void GetAll_FilterByTenantId_ReturnsOnlyMatchingTenant()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success(tenantId: "appA"));
		repo.Create(Failure(tenantId: "appB"));

		var result = repo.GetAll(null, null, tenantId: "appA").ToList();

		Assert.Single(result);
		Assert.Equal("appA", result[0].TenantId);
	}

	[Fact]
	public void Search_ByIsSuccess_False_ReturnsOnlyFailures()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success());
		repo.Create(Failure());
		repo.Create(Failure());

		var result = repo.Search(isSuccess: false).ToList();

		Assert.Equal(2, result.Count);
		Assert.All(result, e => Assert.False(e.IsSuccess));
	}

	[Fact]
	public void Search_ByIsSuccess_True_ReturnsOnlySuccesses()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success());
		repo.Create(Failure());

		var result = repo.Search(isSuccess: true).ToList();

		Assert.Single(result);
		Assert.True(result[0].IsSuccess);
	}

	[Fact]
	public void Search_ByStatusCode_ReturnsOnlyMatchingCode()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Failure(statusCode: 503));
		repo.Create(Failure(statusCode: 404));

		var result = repo.Search(responseStatusCode: 503).ToList();

		Assert.Single(result);
		Assert.Equal(503, result[0].ResponseStatusCode);
	}

	[Fact]
	public void Search_ByUrl_ReturnsOnlyMatchingUrl()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success("https://api.pagamentos.com/charge"));
		repo.Create(Failure("https://api.estoque.com/reservar"));

		var result = repo.Search(url: "pagamentos").ToList();

		Assert.Single(result);
		Assert.Contains("pagamentos", result[0].Url);
	}

	[Fact]
	public void Search_ByTenantId_ReturnsOnlyMatchingTenant()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success(tenantId: "appA"));
		repo.Create(Failure(tenantId: "appB"));

		var result = repo.Search(tenantId: "appA").ToList();

		Assert.Single(result);
		Assert.Equal("appA", result[0].TenantId);
	}

	[Fact]
	public void Search_ByCorrelationId_ReturnsOnlyMatchingLogs()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success(correlationId: "corr-123"));
		repo.Create(Success());

		var result = repo.Search(correlationId: "corr-123").ToList();

		Assert.Single(result);
		Assert.Equal("corr-123", result[0].CorrelationId);
	}

	[Fact]
	public void Search_ByTraceId_ReturnsOnlyMatchingLogs()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success(traceId: "trace-abc"));
		repo.Create(Failure());

		var result = repo.Search(traceId: "trace-abc").ToList();

		Assert.Single(result);
		Assert.Equal("trace-abc", result[0].TraceId);
	}

	[Fact]
	public void Search_ByDateRange_ReturnsLogsWithinRange()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success());

		var result = repo.Search(
			dateStart: DateTime.Now.AddMinutes(-1),
			dateEnd: DateTime.Now.AddMinutes(1)).ToList();

		Assert.Single(result);
	}

	#endregion

	#region Error

	[Fact]
	public void GetById_NonExistingId_ReturnsNull()
	{
		using var context = CreateContext();
		var result = Repo(context).GetById(99999);
		Assert.Null(result);
	}

	[Fact]
	public void GetAll_EmptyDatabase_ReturnsEmptyList()
	{
		using var context = CreateContext();
		var result = Repo(context).GetAll(null, null).ToList();
		Assert.Empty(result);
	}

	[Fact]
	public void Search_NoMatchingUrl_ReturnsEmptyList()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success("https://api.pagamentos.com/charge"));

		var result = repo.Search(url: "nonexistent-service-xyz").ToList();

		Assert.Empty(result);
	}

	[Fact]
	public void Search_DateRangeInFuture_ReturnsEmptyList()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Success());

		var result = repo.Search(
			dateStart: DateTime.Now.AddDays(1),
			dateEnd: DateTime.Now.AddDays(2)).ToList();

		Assert.Empty(result);
	}

	[Fact]
	public void Search_ByStatusCode_NoMatch_ReturnsEmptyList()
	{
		using var context = CreateContext();
		var repo = Repo(context);
		repo.Create(Failure(statusCode: 503));

		var result = repo.Search(responseStatusCode: 404).ToList();

		Assert.Empty(result);
	}

	#endregion
}
