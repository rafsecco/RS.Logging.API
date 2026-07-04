using System.ComponentModel.DataAnnotations;

namespace RS.Logstream.API.ViewModels;

public class LogProcessDetailsViewModel
{
	public LogProcessDetailsViewModel(long logProcessId, LogLevel logLevel, string message, string? stackTrace)
	{
		LogProcessId = logProcessId;
		LogLevel = logLevel;
		Message = message;
		StackTrace = stackTrace;
	}

	[Required]
	public long LogProcessId { get; private set; }
	[Required]
	public LogLevel LogLevel { get; private set; }
	[Required]
	public string Message { get; private set; }
	public string? StackTrace { get; private set; }
}
