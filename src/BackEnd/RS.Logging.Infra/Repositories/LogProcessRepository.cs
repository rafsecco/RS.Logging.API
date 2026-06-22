using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RS.Core.Pagination;
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

	public IEnumerable<LogProcess> GetAll(int? page, int? pageSize, string? tenantId = null)
	{
		var query = _logProcessContext.LogProcess.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		if (page.HasValue && pageSize.HasValue)
		{
			query = query
				.Skip(PaginationHelper.GetSkip(page.Value, pageSize.Value))
				.Take(pageSize.Value);
		}

		var result = query
			.OrderByDescending(o => o.CreatedAt)
			.ToList();

		return result;
	}

	public LogProcess? GetById(ulong id, string? tenantId = null)
	{
		IQueryable<LogProcess> query = _logProcessContext.LogProcess
			.Include(x => x.LorProcessDetailList);

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		return query.FirstOrDefault(p => p.Id == id);
	}

	public IEnumerable<LogProcess> GetAudit(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		ProcessStatus? status,
		int? pageNumber = 1,
		int? pageSize = 10,
		string? tenantId = null)
	{
		var query = _logProcessContext.LogProcess
			.Include(x => x.LorProcessDetailList)
			.AsNoTracking();

		if (dateTimeStart.HasValue)
			query = query.Where(p => p.CreatedAt >= dateTimeStart.Value);

		if (dateTimeEnd.HasValue)
			query = query.Where(p => p.CreatedAt <= dateTimeEnd.Value);

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		query = query.OrderByDescending(o => o.CreatedAt);

		if (pageNumber.HasValue && pageSize.HasValue)
			query = query.Skip(PaginationHelper.GetSkip(pageNumber.Value, pageSize.Value)).Take(pageSize.Value);

		var result = query.ToList();

		if (status.HasValue)
			result = result.Where(p => p.GetStatus() == status.Value).ToList();

		return result;
	}

	public IEnumerable<LogProcess> Search(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		uint? IdProcess,
		string? processName,
		LogLevel? logLevel,
		string? message,
		string? stackTrace,
		int? pageNumber = 1,
		int? pageSize = 10,
		string? tenantId = null,
		string? correlationId = null,
		string? traceId = null,
		string? fullTextQuery = null)
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

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.LogProcess.TenantId == tenantId);

		if (!string.IsNullOrWhiteSpace(correlationId))
			query = query.Where(p => p.CorrelationId == correlationId);

		if (!string.IsNullOrWhiteSpace(traceId))
			query = query.Where(p => p.TraceId == traceId);

		if (!string.IsNullOrWhiteSpace(fullTextQuery))
			query = query.Where(p =>
				EF.Functions.Match(p.Message, fullTextQuery, MySqlMatchSearchMode.NaturalLanguage) > 0 ||
				(p.StackTrace != null && EF.Functions.Match(p.StackTrace, fullTextQuery, MySqlMatchSearchMode.NaturalLanguage) > 0));

		var skip = (pageNumber.HasValue && pageSize.HasValue)
			? PaginationHelper.GetSkip(pageNumber.Value, pageSize.Value)
			: 0;

		query = query
			.Skip(skip)
			.Take(pageSize ?? 10);

		query = query.Include(tb => tb.LogProcess);

		List<LogProcess>? result = query.Select(x => x.LogProcess).ToList();

		return result;
	}
}
