using RS.Core.Entities;

namespace RS.Logging.Domain.LogProcess;

public class LogProcess : BaseEntity
{
	#region Constructors
	protected LogProcess() { }
	public LogProcess(uint pProcessId, string? pName, List<LogProcessDetail>? pLorProcessDetailList = null)
	{
		ProcessId = pProcessId;
		Name = pName;
		LorProcessDetailList = pLorProcessDetailList ?? new List<LogProcessDetail>();
	}
	#endregion

	#region Attributes
	public uint ProcessId { get; private set; }
	public string? Name { get; private set; }
	public List<LogProcessDetail>? LorProcessDetailList { get; set; }
	#endregion
}
