using System.ComponentModel.DataAnnotations;

namespace RS.Logging.API.ViewModels;

public class LogsViewModel
{
	[Required]
	public ulong IdProcess { get; set; }
	[Required]
	public LogLevel LogLevel { get; set; }
	[Required]
	public string Message { get; set; }
	public string Info { get; set; }
	public DateTime DateCreated { get; set; } = DateTime.Now;
}