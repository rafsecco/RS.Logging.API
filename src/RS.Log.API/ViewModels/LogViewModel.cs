using System;

namespace RS.Log.API.ViewModels
{
	public class LogViewModel 
	{
		public DateTime? DateTimeIni { get; set; }
		public DateTime? DateTimeEnd { get; set; } = DateTime.Now;
		public string Message { get; set; }
		public int? PageIndex { get; set; } = 1;
		public int? PageSize { get; set; }
	}
}
