using Microsoft.Extensions.Logging;
using RS.Core.Entities;

namespace RS.Logstream.Domain.LogProcess;

public class LogProcess : BaseEntity
{
	#region Constructors
	protected LogProcess() { }
	public LogProcess(uint pProcessId, string? pName, List<LogProcessDetail>? pLorProcessDetailList = null,
		string? pTenantId = null, string? pCorrelationId = null, string? pTraceId = null)
	{
		ProcessId = pProcessId;
		Name = pName;
		LorProcessDetailList = pLorProcessDetailList ?? new List<LogProcessDetail>();
		TenantId = pTenantId;
		CorrelationId = pCorrelationId;
		TraceId = pTraceId;
	}
	#endregion

	#region Attributes
	public uint ProcessId { get; private set; }
	public string? Name { get; private set; }
	public List<LogProcessDetail>? LorProcessDetailList { get; set; }
	public string? TenantId { get; private set; }
	public string? CorrelationId { get; private set; }
	public string? TraceId { get; private set; }
	#endregion

	#region Methods
	public ProcessStatus GetStatus()
	{
		if (LorProcessDetailList == null || !LorProcessDetailList.Any())
			return ProcessStatus.Success;

		var maxLevel = LorProcessDetailList.Max(d => d.LogLevel);

		return maxLevel switch
		{
			>= LogLevel.Critical => ProcessStatus.Critical,
			>= LogLevel.Error    => ProcessStatus.Error,
			>= LogLevel.Warning  => ProcessStatus.Warning,
			_                    => ProcessStatus.Success
		};
	}
	#endregion
}
