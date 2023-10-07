using Microsoft.Extensions.Logging;
using RS.Core.Entities;

namespace RS.Logging.Domain.Log;

public class Log : BaseEntity
{
	#region Constructors
	protected Log() { }
	public Log(LogLevel pLogLevel, string pMessage, string? pStackTrace = null)
	{
		LogLevel = pLogLevel;
		Message = pMessage;
		StackTrace = pStackTrace;
	}
	#endregion

	#region Attributes
	public LogLevel LogLevel { get; private set; }
	public string Message { get; private set; }
	public string? StackTrace { get; private set; }
	#endregion
}
