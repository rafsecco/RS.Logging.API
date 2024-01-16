using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

	public IEnumerable<LogProcess> GetAll(int? page, int? pageSize)
	{
		var query = _logProcessContext.LogProcess.AsNoTracking();

		if (page.HasValue && pageSize.HasValue)
		{
			query = query
				.Skip((page.Value - 1) * pageSize.Value)
				.Take(pageSize.Value);
		}

		var result = query
			.OrderByDescending(o => o.CreatedAt)
			.ToList();

		return result;
	}

	public LogProcess? GetById(ulong id) =>
		_logProcessContext.LogProcess
		.Include(x => x.LorProcessDetailList)
		.FirstOrDefault(p => p.Id == id);

	public IEnumerable<LogProcess> Search(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		uint? IdProcess,
		string? processName,
		LogLevel? logLevel,
		string? message,
		string? stackTrace,
		int? pageNumber = 1,
		int? pageSize = 10)
	{
		IQueryable<LogProcessDetail> query = _logProcessContext.LogProcessDetails.AsNoTracking();
		query = query.OrderBy(keySelector: o => o.LogProcess.CreatedAt);

		if (dateTimeStart != null)
		{
			query = (dateTimeEnd == null)
				? query.Where(p => p.CreatedAt >= dateTimeStart)
				: query.Where(p => p.CreatedAt >= dateTimeStart && p.CreatedAt <= dateTimeEnd);
		}

		if (IdProcess != null) { query = query.Where(p => p.LogProcessId == IdProcess); }

		if (!string.IsNullOrEmpty(processName?.Trim()))
			query = query.Where(tb => tb.LogProcess.Name.Contains(processName));

		if (logLevel != null)
			query = query.Where(p => p.LogLevel == logLevel);

		if (!string.IsNullOrEmpty(message?.Trim()))
			query = query.Where(p => p.Message.Contains(message));

		if (!string.IsNullOrEmpty(stackTrace?.Trim()))
			query = query.Where(p => p.StackTrace.Contains(stackTrace));

		query = query
			.Skip((pageNumber - 1) * pageSize ?? 0)
			.Take(pageSize ?? 10);

		query = query.Include(tb => tb.LogProcess);

		List<LogProcess>? result = query.Select(x => x.LogProcess).ToList();

		return result;
	}
}
