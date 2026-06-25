using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Providers;
using RS.Logstream.Infra.Repositories;
using Xunit;

namespace RS.Logstream.Tests.Repositories;

public class LogProcessRepositoryTests
{
	private static RSLoggingDbContext CreateContext() =>
		new(new DbContextOptionsBuilder<RSLoggingDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options, new MariaDbColumnTypes());

	private static LogProcessRepository Repo(RSLoggingDbContext ctx) =>
		new(ctx, new LikeFullTextProvider());

	#region Success

	[Fact]
	public void CreateLogProcess_ValidProcess_ReturnsGeneratedId()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var process = new LogProcess(1, "Payment Process");

		// Act
		var id = repository.CreateLogProcess(process);

		// Assert
		Assert.True(id > 0);
	}

	[Fact]
	public void CreateLogProcessDetail_ValidDetail_ReturnsTrue()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var processId = repository.CreateLogProcess(new LogProcess(1, "Payment Process"));
		var detail = new LogProcessDetail(processId, LogLevel.Information, "Step 1 completed");

		// Act
		var result = repository.CreateLogProcessDetail(detail);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void GetById_ExistingId_ReturnsProcessWithDetails()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var processId = repository.CreateLogProcess(new LogProcess(1, "Order Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 1: validate"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 2: persist"));

		// Act
		var result = repository.GetById(processId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Order Process", result.Name);
		Assert.NotNull(result.LorProcessDetailList);
		Assert.Equal(2, result.LorProcessDetailList.Count);
	}

	[Fact]
	public void GetAll_WithProcesses_ReturnsList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.CreateLogProcess(new LogProcess(1, "Process A"));
		repository.CreateLogProcess(new LogProcess(2, "Process B"));
		repository.CreateLogProcess(new LogProcess(3, "Process C"));

		// Act
		var result = repository.GetAll(null, null).ToList();

		// Assert
		Assert.Equal(3, result.Count);
	}

	[Fact]
	public void GetAudit_NoFilter_ReturnsAllProcessesWithStatus()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		var successId = repository.CreateLogProcess(new LogProcess(1, "Success Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(successId, LogLevel.Information, "All good"));

		var errorId = repository.CreateLogProcess(new LogProcess(2, "Error Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(errorId, LogLevel.Error, "Failed to connect"));

		// Act
		var result = repository.GetAudit(null, null, null, null, null).ToList();

		// Assert
		Assert.Equal(2, result.Count);
		Assert.Contains(result, p => p.GetStatus() == ProcessStatus.Success);
		Assert.Contains(result, p => p.GetStatus() == ProcessStatus.Error);
	}

	[Fact]
	public void GetAudit_FilterByStatusError_ReturnsOnlyErrorProcesses()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		var successId = repository.CreateLogProcess(new LogProcess(1, "Success Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(successId, LogLevel.Information, "Step completed"));

		var errorId = repository.CreateLogProcess(new LogProcess(2, "Error Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(errorId, LogLevel.Error, "Database connection failed"));

		// Act
		var result = repository.GetAudit(null, null, ProcessStatus.Error, null, null).ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal(ProcessStatus.Error, result[0].GetStatus());
		Assert.Equal("Error Process", result[0].Name);
	}

	[Fact]
	public void GetAudit_FilterByStatusSuccess_ReturnsOnlySuccessProcesses()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		var successId = repository.CreateLogProcess(new LogProcess(1, "Success Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(successId, LogLevel.Information, "Completed"));

		var errorId = repository.CreateLogProcess(new LogProcess(2, "Error Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(errorId, LogLevel.Critical, "System failure"));

		// Act
		var result = repository.GetAudit(null, null, ProcessStatus.Success, null, null).ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal(ProcessStatus.Success, result[0].GetStatus());
		Assert.Equal("Success Process", result[0].Name);
	}

	[Fact]
	public void GetAudit_FilterByDateRange_ReturnsOnlyProcessesInRange()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.CreateLogProcess(new LogProcess(1, "Recent Process"));

		var dateStart = DateTime.Now.AddMinutes(-1);
		var dateEnd = DateTime.Now.AddMinutes(1);

		// Act
		var result = repository.GetAudit(dateStart, dateEnd, null, null, null).ToList();

		// Assert
		Assert.Single(result);
	}

	[Fact]
	public void GetAll_FilterByTenantId_ReturnsOnlyMatchingTenant()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.CreateLogProcess(new LogProcess(1, "Process A", pTenantId: "tenantA"));
		repository.CreateLogProcess(new LogProcess(2, "Process B", pTenantId: "tenantB"));

		// Act
		var result = repository.GetAll(null, null, tenantId: "tenantA").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("Process A", result[0].Name);
	}

	[Fact]
	public void GetById_DifferentTenant_ReturnsNull()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var processId = repository.CreateLogProcess(new LogProcess(1, "Process A", pTenantId: "tenantA"));

		// Act
		var result = repository.GetById(processId, tenantId: "tenantB");

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void GetAudit_FilterByTenantId_ReturnsOnlyMatchingTenant()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.CreateLogProcess(new LogProcess(1, "Process A", pTenantId: "tenantA"));
		repository.CreateLogProcess(new LogProcess(2, "Process B", pTenantId: "tenantB"));

		// Act
		var result = repository.GetAudit(null, null, null, null, null, tenantId: "tenantA").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("Process A", result[0].Name);
	}

	[Fact]
	public void Search_ByTenantId_ReturnsOnlyMatchingTenant()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		var processAId = repository.CreateLogProcess(new LogProcess(1, "Process A", pTenantId: "tenantA"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processAId, LogLevel.Information, "Step A"));

		var processBId = repository.CreateLogProcess(new LogProcess(2, "Process B", pTenantId: "tenantB"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processBId, LogLevel.Information, "Step B"));

		// Act
		var result = repository.Search(null, null, null, null, null, null, null, null, null, tenantId: "tenantA").ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal("Process A", result[0].Name);
	}

	[Fact]
	public void Search_ByCorrelationId_ReturnsOnlyMatchingProcess()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var processId = repository.CreateLogProcess(new LogProcess(1, "Process A"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 1", pCorrelationId: "corr-123"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 2"));

		// Act
		var result = repository.Search(null, null, null, null, null, null, null, null, null, correlationId: "corr-123").ToList();

		// Assert
		Assert.Single(result);
	}

	[Fact]
	public void Search_ByTraceId_ReturnsOnlyMatchingProcess()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var processId = repository.CreateLogProcess(new LogProcess(1, "Process A"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 1", pTraceId: "trace-abc"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 2"));

		// Act
		var result = repository.Search(null, null, null, null, null, null, null, null, null, traceId: "trace-abc").ToList();

		// Assert
		Assert.Single(result);
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
	public void GetAudit_FilterByStatusError_NoErrorProcesses_ReturnsEmptyList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);

		var successId = repository.CreateLogProcess(new LogProcess(1, "Success Process"));
		repository.CreateLogProcessDetail(new LogProcessDetail(successId, LogLevel.Information, "All good"));

		// Act
		var result = repository.GetAudit(null, null, ProcessStatus.Error, null, null).ToList();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void GetAudit_DateRangeInFuture_ReturnsEmptyList()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		repository.CreateLogProcess(new LogProcess(1, "Old Process"));

		var dateStart = DateTime.Now.AddDays(1);
		var dateEnd = DateTime.Now.AddDays(2);

		// Act
		var result = repository.GetAudit(dateStart, dateEnd, null, null, null).ToList();

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void GetAudit_ProcessWithMultipleSteps_CorrectlyIdentifiesFailingStep()
	{
		// Arrange
		using var context = CreateContext();
		var repository = Repo(context);
		var processId = repository.CreateLogProcess(new LogProcess(1, "Order Processing"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 1: order received"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 2: payment validated"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Error, "Step 3: stock reservation failed"));
		repository.CreateLogProcessDetail(new LogProcessDetail(processId, LogLevel.Information, "Step 4: notification sent"));

		// Act
		var result = repository.GetAudit(null, null, ProcessStatus.Error, null, null).ToList();
		var failingStep = result[0].LorProcessDetailList!.First(d => d.LogLevel == LogLevel.Error);

		// Assert
		Assert.Single(result);
		Assert.Equal(ProcessStatus.Error, result[0].GetStatus());
		Assert.Contains("Step 3", failingStep.Message);
	}

	#endregion
}
