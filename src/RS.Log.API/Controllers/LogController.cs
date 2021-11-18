using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RS.Log.API.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using RS.Log.API.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RS.Log.API.Controllers
{
	[ApiController]
	[Route("{tenant}/[controller]")]
	public class LogController : ControllerBase
	{
		private readonly ILogger<LogController> _logger;
		private readonly IConfiguration _config;

		public LogController(ILogger<LogController> logger, IConfiguration config)
		{
			_logger = logger;
			_config = config;
		}

		[HttpGet]
		[Route("Search")]
		public async Task<IActionResult> GetRange([FromServices] LogsContext db, [FromQuery] LogViewModel model)
		{
			var objReturn = new PagedResult<Domain.Log>();
			objReturn.PageIndex = model.PageIndex ?? 1;
			objReturn.PageSize = model.PageSize ?? _config.GetValue<int>("AppSettings:PageSize");
			
			var query = db.Logs.AsNoTracking().AsQueryable();

			#region Search
			if (!string.IsNullOrEmpty(model.Message?.Trim()))
				query = query.Where(w => w.Message.Contains(model.Message));

			if (model.DateTimeIni.HasValue)
				query = query.Where(w => w.DateCreated >= model.DateTimeIni && w.DateCreated <= model.DateTimeEnd);
			#endregion

			objReturn.TotalResults = await query.CountAsync();

			// Paging
			query = query
				.Skip(objReturn.PageSize * (objReturn.PageIndex - 1))
				.Take(objReturn.PageSize)
				.OrderBy(x => x.DateCreated);

			objReturn.List = await query.ToListAsync();

			return Ok(objReturn);
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
