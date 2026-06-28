using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RS.Logstream.Domain.Log;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Providers;
using RS.Logstream.Infra.Repositories;
using Xunit;

namespace RS.Logstream.Tests.Repositories;

public class LogRepositoryTests
{
	private static RSLogstreamDbContext CreateContext() =>
		new(new DbContextOptionsBuilder<RSLogstreamDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options, new MariaDbColumnTypes());

	private static LogRepository Repo(RSLogstreamDbContext ctx) =>
		new(ctx, new LikeFullTextProvider());

	#region Success

	[Fact]
	public void Create_ValidLog_ReturnsTrue()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var log = new Log(LogLevel.Information, "Application started");

		// Act
		var result = repository.Create(log);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void Create_ValidLog_PersistsToDatabase()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var log = new Log(LogLevel.Information, "Application started");

		// Act
		repository.Create(log);
		var saved = context.Logs.FirstOrDefault(l => l.Id == log.Id);

		// Assert
		Assert.NotNull(saved);
		Assert.Equal("Application started", saved.Message);
	}

	[Fact]
	public void GetById_ExistingId_ReturnsLog()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var log = new Log(LogLevel.Warning, "Disk usage high");
		repository.Create(log);

		// Act
		var result = repository.GetById(log.Id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(LogLevel.Warning, result.LogLevel);
		Assert.Equal("Disk usage high", result.Message);
	}

	[Fact]
	public void GetAll_WithLogs_ReturnsPaginatedList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Log 1"));
		repository.Create(new Log(LogLevel.Information, "Log 2"));
		repository.Create(new Log(LogLevel.Information, "Log 3"));

		// Act
		var result = repository.GetAll(page: 1, pageSize: 2).ToList();

		// Assert
		Assert.Equal(2, result.Count);
	}

	[Fact]
	public void Search_ByLogLevel_ReturnsOnlyMatchingLogs()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Info log"));
		repository.Create(new Log(LogLevel.Error, "Error log 1"));
		repository.Create(new Log(LogLevel.Error, "Error log 2"));

		// Act
		var result = repository.Search(null, null, LogLevel.Error, null, null, null).ToList();

		// Assert
		Assert.Equal(2, result.Count);
		Assert.All(result, l => Assert.Equal(LogLevel.Error, l.LogLevel));
	}

	[Fact]
	public void Search_ByMessage_ReturnsOnlyMatchingLogs()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Error, "NullReferenceException in PaymentService"));
		repository.Create(new Log(LogLevel.Error, "Timeout in OrderService"));
		repository.Create(new Log(LogLevel.Information, "Request completed"));

		// Act
		var result = repository.Search(null, null, null, "Exception", null, null).ToList();

		// Assert
		Assert.Single(result);
		Assert.Contains("Exception", result[0].Message);
	}

	[Fact]
	public void Search_ByDateRange_ReturnsLogsWithinRange()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Recent log"));

		var dateStart = DateTime.Now.AddMinutes(-1);
		var dateEnd = DateTime.Now.AddMinutes(1);

		// Act
		var result = repository.Search(dateStart, dateEnd, null, null, null, null).ToList();

		// Assert
		Assert.Single(result);
	}

	[Fact]
	public void GetAll_FilterByTenantId_ReturnsOnlyMatchingTenant()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Tenant A log", pTenantId: "tenantA"));
		repository.Create(new Log(LogLevel.Information, "Tenant B log", pTenantId: "tenantB"));

		// Act
		var result = repository.GetAll(null, null, tenantId: "tenantA").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("tenantA", result[0].TenantId);
	}

	[Fact]
	public void GetById_DifferentTenant_ReturnsNull()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var log = new Log(LogLevel.Information, "Tenant A log", pTenantId: "tenantA");
		repository.Create(log);

		// Act
		var result = repository.GetById(log.Id, tenantId: "tenantB");

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void Search_ByTenantId_ReturnsOnlyMatchingTenant()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Tenant A log", pTenantId: "tenantA"));
		repository.Create(new Log(LogLevel.Information, "Tenant B log", pTenantId: "tenantB"));

		// Act
		var result = repository.Search(null, null, null, null, null, null, tenantId: "tenantA").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("tenantA", result[0].TenantId);
	}

	[Fact]
	public void Search_ByCorrelationId_ReturnsOnlyMatchingLogs()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Log with correlation", pCorrelationId: "corr-123"));
		repository.Create(new Log(LogLevel.Information, "Log without correlation"));

		// Act
		var result = repository.Search(null, null, null, null, null, null, correlationId: "corr-123").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("corr-123", result[0].CorrelationId);
	}

	[Fact]
	public void Search_ByTraceId_ReturnsOnlyMatchingLogs()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Log with trace", pTraceId: "trace-abc"));
		repository.Create(new Log(LogLevel.Information, "Log without trace"));

		// Act
		var result = repository.Search(null, null, null, null, null, null, traceId: "trace-abc").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("trace-abc", result[0].TraceId);
	}

	#endregion

	#region Error

	[Fact]
	public void GetById_NonExistingId_ReturnsNull()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		// Act
		var result = repository.GetById(99999);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void GetAll_EmptyDatabase_ReturnsEmptyList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		// Act
		var result = repository.GetAll(null, null).ToList();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void Search_NoMatchingMessage_ReturnsEmptyList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Hello world"));

		// Act
		var result = repository.Search(null, null, null, "nonexistent_xyz", null, null).ToList();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void Search_DateRangeInFuture_ReturnsEmptyList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Past log"));

		var dateStart = DateTime.Now.AddDays(1);
		var dateEnd = DateTime.Now.AddDays(2);

		// Act
		var result = repository.Search(dateStart, dateEnd, null, null, null, null).ToList();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void Search_NoMatchingLogLevel_ReturnsEmptyList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.Create(new Log(LogLevel.Information, "Info log"));

		// Act
		var result = repository.Search(null, null, LogLevel.Critical, null, null, null).ToList();

		// Assert
		Assert.Empty(result);
	}

	#endregion
}
