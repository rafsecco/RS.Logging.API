using RS.Core.Entities;
using RS.Logging.API.Domain;
using System.Reflection;

namespace RS.Logging.API.ViewModels;

public class SearchLogsViewModel : PagedModel<Log>
{
	public DateTime? DateTimeIni { get; set; } = DateTime.Now.Date;
	public DateTime? DateTimeFim { get; set; }
	public string? Message { get; set; }
	public LogLevel? LogLevel { get; set; }
	public ulong? IdProcess { get; set; }
}
