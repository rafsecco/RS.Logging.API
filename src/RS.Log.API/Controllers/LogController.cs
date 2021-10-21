using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RS.Log.API.Data;

namespace RS.Log.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class LogController : ControllerBase
	{
		private readonly ILogger<LogController> _logger;

		public LogController(ILogger<LogController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<Domain.Log> Get([FromServices]ApplicationContext db)
		{
			var logs = db.Logs.ToArray();
			return logs;
		}
	}
}
