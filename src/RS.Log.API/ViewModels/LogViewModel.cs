using System;
using System.ComponentModel.DataAnnotations;

namespace RS.Log.API.ViewModels
{
	public class LogViewModel
	{
		public DateTime? DateTimeIni { get; set; }
		public DateTime? DateTimeEnd { get; set; }
		public string Message { get; set; }
	}
}
