using Microsoft.Extensions.Logging;
using RS.Core.Entities;

namespace RS.Logging.Domain.LogProcess;

public class LogProcessDetail : BaseEntity
{
	#region Constructors
	protected LogProcessDetail() { }
	public LogProcessDetail(ulong pLogProcessId, LogLevel pLogLevel, string pMessage, string? pStackTrace = null)
	{
		LogProcessId = pLogProcessId;
		LogLevel = pLogLevel;
		Message = pMessage;
		StackTrace = pStackTrace;
	}
	#endregion

	#region Attributes
	public ulong LogProcessId { get; set; } // Required foreign key property
	public LogLevel LogLevel { get; private set; }
	public string Message { get; private set; }
	public string? StackTrace { get; private set; }
	public LogProcess LogProcess { get; set; } = null!; // Required reference navigation to principal
	#endregion
}
