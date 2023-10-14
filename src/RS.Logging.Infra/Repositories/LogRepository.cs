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

	public Log GetById(ulong id) => _logContext.Logs.First(p => p.Id == id);

	public IEnumerable<Log> GetAll(int? page, int? pageSize)
	{
		if (page.HasValue && pageSize.HasValue)
		{
			return _logContext.Logs.AsNoTracking()
				.Skip((page.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToList();
		}

		return _logContext.Logs.AsNoTracking()
			.OrderByDescending(o => o.CreatedAt)
			.ToList();
	}

	public IEnumerable<Log> GetSearch(int? page = null, int? pageSize = null, DateTime? DateTimeIni = null, DateTime? DateTimeFim = null, LogLevel? LogLevel = null, string? Message = null)
	{
		DateTimeIni ??= DateTime.Now;

		IQueryable<Log> query = _logContext.Logs.AsNoTracking();

		if (DateTimeFim.HasValue) { query = query.Where(p => p.CreatedAt >= DateTimeIni && p.CreatedAt <= DateTimeFim); }
		else { query = query.Where(p => p.CreatedAt >= DateTimeIni); }

		if (!LogLevel.HasValue) { query = query.Where(p => p.LogLevel == LogLevel); }
		if (!string.IsNullOrEmpty(Message)) { query = query.Where(s => s.Message.Contains(Message)); }

		query = query.OrderByDescending(o => o.CreatedAt);

		//var totalResults = query.Count();
		if (page.HasValue && pageSize.HasValue)
		{
			query = query
				.Skip(pageSize.Value * (page.Value - 1))
				.Take(pageSize.Value);
		}

		return query.ToList();
	}
}
