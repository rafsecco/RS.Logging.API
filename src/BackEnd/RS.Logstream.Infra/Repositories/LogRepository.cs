using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RS.Core.Pagination;
using RS.Logstream.Domain.Log;
using RS.Logstream.Domain.Log.Contracts;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Providers;

namespace RS.Logstream.Infra.Repositories;

public class LogRepository : ILogRepository
{
	private readonly RSLoggingDbContext _logContext;
	private readonly IFullTextSearchProvider _fullText;

	public LogRepository(RSLoggingDbContext logContext, IFullTextSearchProvider fullText)
	{
		_logContext = logContext;
		_fullText = fullText;
	}

	public bool Create(Log log)
	{
		_logContext.Logs.Add(log);
		var changes = _logContext.SaveChanges();
		return changes > 0;
	}

	public Log? GetById(ulong id, string? tenantId = null)
	{
		IQueryable<Log> query = _logContext.Logs;

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		return query.FirstOrDefault(p => p.Id == id);
	}

	public IEnumerable<Log> GetAll(int? page, int? pageSize, string? tenantId = null)
	{
		var query = _logContext.Logs.AsNoTracking();

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

	public IEnumerable<Log> Search(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		LogLevel? logLevel,
		string? message,
		int? pageNumber,
		int? pageSize,
		string? tenantId = null,
		string? correlationId = null,
		string? traceId = null,
		string? fullTextQuery = null)
	{
		IQueryable<Log> query = _logContext.Logs.AsNoTracking();
		query = query.OrderBy(o => o.CreatedAt);

		if (dateTimeStart != null)
		{
			query = (dateTimeEnd == null)
				? query.Where(p => p.CreatedAt >= dateTimeStart)
				: query.Where(p => p.CreatedAt >= dateTimeStart && p.CreatedAt <= dateTimeEnd);
		}

		if (logLevel != null)
			query = query.Where(p => p.LogLevel == logLevel);

		if (!string.IsNullOrEmpty(message?.Trim()))
			query = query.Where(p => p.Message.Contains(message));

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		if (!string.IsNullOrWhiteSpace(correlationId))
			query = query.Where(p => p.CorrelationId == correlationId);

		if (!string.IsNullOrWhiteSpace(traceId))
			query = query.Where(p => p.TraceId == traceId);

		if (!string.IsNullOrWhiteSpace(fullTextQuery))
			query = _fullText.ApplyToLogs(query, fullTextQuery);

		if (pageNumber is not null && pageSize is not null)
		{
			query = query
				.Skip(PaginationHelper.GetSkip(pageNumber.Value, pageSize.Value))
				.Take(pageSize.Value);
		}

		return query.ToList();
	}
}
