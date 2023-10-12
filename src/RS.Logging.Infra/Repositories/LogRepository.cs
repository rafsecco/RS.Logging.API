using Microsoft.EntityFrameworkCore;
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
}
