using System.ComponentModel.DataAnnotations;

namespace RS.Logging.API.ViewModels;

public class LogsViewModel
{
	public LogsViewModel(LogLevel logLevel, string message, string? stackTrace)
	{
		LogLevel = logLevel;
		Message = message;
		StackTrace = stackTrace;
	}

	[Required]
	public LogLevel LogLevel { get; set; }
	//[Required(ErrorMessage = "O campo {0} é obrigatório")]
	[Required]
	public string Message { get; set; }
	public string? StackTrace { get; set; }
}
