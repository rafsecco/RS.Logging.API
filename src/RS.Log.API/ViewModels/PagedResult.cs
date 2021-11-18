using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Log.API.ViewModels
{
	public class PagedResult<T> where T : class
	{
		public IEnumerable<T> List { get; set; }
		public int TotalResults { get; set; }
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; }
	}
}
