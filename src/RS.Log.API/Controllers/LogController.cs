using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RS.Log.API.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using RS.Log.API.ViewModels;

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
		[Route("Search")]
		public IEnumerable<Domain.Log> GetRange([FromServices] LogsContext db, [FromQuery] LogViewModel model)
		{
			var logs = db.Logs.Where(f => (
				(model.DateTimeIni == null || f.DateCreated >= model.DateTimeIni.Value)
					&& (model.DateTimeEnd == null || f.DateCreated <= model.DateTimeEnd.Value)
				) && f.Message.Contains(model.Message ?? string.Empty)
			).ToArray();

			return logs;
		}

		[HttpPost]
		public async Task<IActionResult> PostAsync([FromServices] LogsContext db, [FromBody] Domain.Log model)
		{
			Domain.Log objLog = new() { Message = model.Message, StackTrace = model.StackTrace };

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
