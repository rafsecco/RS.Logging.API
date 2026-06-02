using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RS.Logging.Domain.Log;
using RS.Logging.Infra.Contexts;
using RS.Logging.Infra.Repositories;
using Xunit;

namespace RS.Logging.Tests.Repositories;

public class LogRepositoryTests
{
    private static RSLoggingDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<RSLoggingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    #region Success

    [Fact]
    public void Create_ValidLog_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
        repository.Create(new Log(LogLevel.Information, "Recent log"));

        var dateStart = DateTime.Now.AddMinutes(-1);
        var dateEnd = DateTime.Now.AddMinutes(1);

        // Act
        var result = repository.Search(dateStart, dateEnd, null, null, null, null).ToList();

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
        var repository = new LogRepository(context);

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
        var repository = new LogRepository(context);

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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
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
        var repository = new LogRepository(context);
        repository.Create(new Log(LogLevel.Information, "Info log"));

        // Act
        var result = repository.Search(null, null, LogLevel.Critical, null, null, null).ToList();

        // Assert
        Assert.Empty(result);
    }

    #endregion
}
