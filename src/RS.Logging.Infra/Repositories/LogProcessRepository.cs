using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.LogProcess;
using RS.Logging.Domain.LogProcess.Contracts;
using RS.Logging.Infra.Contexts;

namespace RS.Logging.Infra.Repositories;

public class LogProcessRepository : ILogProcessRepository
{
	private readonly RSLoggingDbContext _logProcessContext;

	public LogProcessRepository(RSLoggingDbContext logProcessContext)
	{
		_logProcessContext = logProcessContext;
	}

	public ulong CreateLogProcess(LogProcess logProcess)
	{
		_logProcessContext.LogProcess.Add(logProcess);
		var changes = _logProcessContext.SaveChanges();
		ulong lastId = logProcess.Id;
		return lastId;
	}

	public bool CreateLogProcessDetail(LogProcessDetail logProcessDetails)
	{
		_logProcessContext.LogProcessDetails.Add(logProcessDetails);
		var changes = _logProcessContext.SaveChanges();
		return changes > 0;
	}

	public LogProcess? GetById(ulong id) => _logProcessContext.LogProcess.FirstOrDefault(p => p.Id == id, null);

	public IEnumerable<LogProcess> GetAll(int? page, int? pageSize)
	{
		if (page.HasValue && pageSize.HasValue)
		{
			return _logProcessContext.LogProcess.AsNoTracking()
				.OrderByDescending(o => o.CreatedAt)
				.Skip((page.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToList();
		}

		return _logProcessContext.LogProcess.AsNoTracking()
			.OrderByDescending(o => o.CreatedAt)
			.ToList();
	}
}
