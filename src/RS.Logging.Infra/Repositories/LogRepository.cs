using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RS.Logging.Domain.Log;
using RS.Logging.Domain.Log.Contracts;
using RS.Logging.Infra.Contexts;

namespace RS.Logging.Infra.Repositories;

public class LogRepository : ILogRepository
{
	private readonly RSLoggingDbContext _logContext;


	public LogRepository(RSLoggingDbContext logContext)
	{
		_logContext = logContext;
	}

	public bool Create(Log log)
	{
		_logContext.Logs.Add(log);
		var changes = _logContext.SaveChanges();
		return changes > 0;
	}

	public Log? GetById(ulong id)
	{
		return _logContext.Logs.FirstOrDefault(p => p.Id == id);
	}

	public IEnumerable<Log> GetAll()
	{
		var logList = _logContext.Logs.AsNoTracking()
			.OrderByDescending(o => o.CreatedAt)
			.ToList();
		return logList;
	}

	public IEnumerable<Log> Search(DateTime? dateTimeStart, DateTime? dateTimeEnd, LogLevel? logLevel, string? message, int? pageNumber, int? pageSize)
	{
		IQueryable<Log> query = _logContext.Logs.AsNoTracking();
		query = query.OrderBy(o => o.CreatedAt);

		if (dateTimeStart != null)
		{
			query = (dateTimeEnd == null)
				? query.Where(p => p.CreatedAt >= dateTimeStart)
				: query.Where(p => p.CreatedAt >= dateTimeStart && p.CreatedAt <= dateTimeEnd);
		}

		//if (IdProcess != null) { query = query.Where(p => p.Id == IdProcess); }

		if (logLevel != null)
		{
			query = query.Where(p => p.LogLevel == logLevel);
		}

		if (!string.IsNullOrEmpty(message?.Trim()))
		{
			query = query.Where(p => p.Message.Contains(message));
		}

		if (pageNumber is not null && pageSize is not null)
		{
			var registerSkipped = pageNumber == 1
				? 0
				: (pageNumber * pageSize - pageSize);

			query = query
				.Skip((int)registerSkipped)
				.Take((int)pageSize);
		}

		return query.ToList();
	}
}
