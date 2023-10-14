using Microsoft.AspNetCore.Mvc;

namespace RS.Logging.API.ViewModels;

[BindProperties]
public class SearchLogsViewModel //: PagedModel<Log>
{
	//public int? page { get; set; }
	//public int? pageSize { get; set; }
	public DateTime? DateTimeIni { get; set; } = DateTime.Now.Date;
	public DateTime? DateTimeFim { get; set; }
	public string? Message { get; set; }
	public LogLevel? LogLevel { get; set; }
	public ulong? IdProcess { get; set; }
}
