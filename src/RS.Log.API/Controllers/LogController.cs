using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RS.Log.API.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace RS.Log.API.Controllers
{
	[ApiController]
	[Route("{tenant}/[controller]")]
	public class LogController : ControllerBase
	{
		private readonly ILogger<LogController> _logger;

		public LogController(ILogger<LogController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<Domain.Log> Get([FromServices] ApplicationContext db)
		{
			var logs = db.Logs.ToArray();
			return logs;
		}

		[HttpPost]
		public async Task<IActionResult> PostAsync(
			[FromBody] Domain.Log model,
			[FromServices] ApplicationContext db)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var objLog = new Domain.Log();
			objLog.Message = model.Message;
			objLog.StackTrace = model.StackTrace;
			objLog.TenantId = model.TenantId;

			try
			{
				await db.Logs.AddAsync(objLog);
				await db.SaveChangesAsync();
				return Ok(objLog);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
