using System.ComponentModel.DataAnnotations;

namespace RS.Logging.API.ViewModels;

public class LogProcessViewModel
{
	public LogProcessViewModel(uint processId, string? name)
	{
		ProcessId = processId;
		Name = name;
	}

	[Required]
	public uint ProcessId { get; private set; }
	public string? Name { get; private set; }

	//public List<LogProcessDetailsViewModel> LorProcessDetailList { get; set; }
}
