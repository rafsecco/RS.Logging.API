using RS.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RS.Logging.API.Domain;

public class Log : BaseEntity
{
	#region Attributes
	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public ulong IdProcess { get; private set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public LogLevel LogLevel { get; private set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public string Message { get; private set; }

	public string Info { get; private set; }

	#endregion

	#region Constructors
	protected Log() { }

	public Log(ulong pIdProcess, LogLevel pLogLevel, string pMessage, string pInfo, DateTime pDateTime)
	{
		IdProcess = pIdProcess;
		LogLevel = pLogLevel;
		Message = pMessage;
		Info = pInfo;
		DateCreated = pDateTime;
	}
	#endregion

}

