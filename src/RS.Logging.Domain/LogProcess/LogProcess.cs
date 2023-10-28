using RS.Core.Entities;

namespace RS.Logging.Domain.LogProcess;

public class LogProcess : BaseEntity
{
	#region Constructors
	protected LogProcess() { }
	public LogProcess(uint pProcessId, string? pName, List<LogProcessDetails>? pLorProcessDetailList)
	{
		ProcessId = pProcessId;
		Name = pName;
		LorProcessDetailList = pLorProcessDetailList ?? new List<LogProcessDetails>();
	}
	#endregion

	#region Attributes
	public uint ProcessId { get; private set; }
	public string? Name { get; private set; }
	public List<LogProcessDetails> LorProcessDetailList { get; set; }
	#endregion
}
