using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Log.API.Domain
{
	public class Log
	{
		public int Id { get; set; }
		public string Project { get; set; }
		public string Message { get; set; }
		public string StackTrace { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.Now;
	}
}
