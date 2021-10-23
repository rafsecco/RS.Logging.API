using System;
using System.ComponentModel.DataAnnotations;

namespace RS.Log.API.ViewModels
{
	public class LogViewModel
	{
		[Required]
		public string TenantId { get; set; }
		public DateTime DateTimeIni { get; set; }
		public DateTime DateTimeEnd { get; set; }
		public string Message { get; set; }
	}
}
