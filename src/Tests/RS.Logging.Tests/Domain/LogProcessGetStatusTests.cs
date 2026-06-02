using Microsoft.Extensions.Logging;
using RS.Logging.Domain.LogProcess;
using Xunit;

namespace RS.Logging.Tests.Domain;

public class LogProcessGetStatusTests
{
    private static LogProcess CreateProcess(List<LogProcessDetail>? details = null)
        => new(1, "Test Process", details);

    private static LogProcessDetail CreateDetail(LogLevel level)
        => new(0, level, "Test message");

    #region Success

    [Fact]
    public void GetStatus_NullDetailList_ReturnsSuccess()
    {
        // Arrange
        var process = CreateProcess(null);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Success, status);
    }

    [Fact]
    public void GetStatus_EmptyDetailList_ReturnsSuccess()
    {
        // Arrange
        var process = CreateProcess([]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Success, status);
    }

    [Fact]
    public void GetStatus_AllInformationAndBelow_ReturnsSuccess()
    {
        // Arrange
        var process = CreateProcess([
            CreateDetail(LogLevel.Trace),
            CreateDetail(LogLevel.Debug),
            CreateDetail(LogLevel.Information)
        ]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Success, status);
    }

    #endregion

    #region Error

    [Fact]
    public void GetStatus_HasWarning_ReturnsWarning()
    {
        // Arrange
        var process = CreateProcess([
            CreateDetail(LogLevel.Information),
            CreateDetail(LogLevel.Warning)
        ]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Warning, status);
    }

    [Fact]
    public void GetStatus_HasError_ReturnsError()
    {
        // Arrange
        var process = CreateProcess([
            CreateDetail(LogLevel.Information),
            CreateDetail(LogLevel.Error)
        ]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Error, status);
    }

    [Fact]
    public void GetStatus_HasCritical_ReturnsCritical()
    {
        // Arrange
        var process = CreateProcess([
            CreateDetail(LogLevel.Error),
            CreateDetail(LogLevel.Critical)
        ]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Critical, status);
    }

    [Fact]
    public void GetStatus_ErrorAndWarningMixed_ReturnsError()
    {
        // Arrange
        var process = CreateProcess([
            CreateDetail(LogLevel.Warning),
            CreateDetail(LogLevel.Error),
            CreateDetail(LogLevel.Information)
        ]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Error, status);
    }

    [Fact]
    public void GetStatus_CriticalAndErrorMixed_ReturnsCritical()
    {
        // Arrange
        var process = CreateProcess([
            CreateDetail(LogLevel.Error),
            CreateDetail(LogLevel.Critical),
            CreateDetail(LogLevel.Warning)
        ]);

        // Act
        var status = process.GetStatus();

        // Assert
        Assert.Equal(ProcessStatus.Critical, status);
    }

    #endregion
}
