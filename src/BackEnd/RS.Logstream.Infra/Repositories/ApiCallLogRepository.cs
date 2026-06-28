using Microsoft.EntityFrameworkCore;
using RS.Core.Pagination;
using RS.Logstream.Domain.ApiCall;
using RS.Logstream.Domain.ApiCall.Contracts;
using RS.Logstream.Infra.Contexts;

namespace RS.Logstream.Infra.Repositories;

public class ApiCallLogRepository : IApiCallLogRepository
{
	private readonly RSLogstreamDbContext _context;

	public ApiCallLogRepository(RSLogstreamDbContext context)
	{
		_context = context;
	}

	public bool Create(ApiCallLog apiCallLog)
	{
		_context.ApiCallLogs.Add(apiCallLog);
		return _context.SaveChanges() > 0;
	}

	public ApiCallLog? GetById(ulong id, string? tenantId = null)
	{
		IQueryable<ApiCallLog> query = _context.ApiCallLogs;

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		return query.FirstOrDefault(p => p.Id == id);
	}

	public IEnumerable<ApiCallLog> GetAll(int? page, int? pageSize, string? tenantId = null)
	{
		IQueryable<ApiCallLog> query = _context.ApiCallLogs.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		if (page.HasValue && pageSize.HasValue)
			query = query.Skip(PaginationHelper.GetSkip(page.Value, pageSize.Value)).Take(pageSize.Value);

		return query.OrderByDescending(o => o.CreatedAt).ToList();
	}

	public IEnumerable<ApiCallLog> Search(
		DateTime? dateStart = null,
		DateTime? dateEnd = null,
		bool? isSuccess = null,
		int? responseStatusCode = null,
		string? url = null,
		int? pageNumber = null,
		int? pageSize = null,
		string? tenantId = null,
		string? correlationId = null,
		string? traceId = null)
	{
		IQueryable<ApiCallLog> query = _context.ApiCallLogs.AsNoTracking();

		if (dateStart != null)
			query = dateEnd == null
				? query.Where(p => p.CreatedAt >= dateStart)
				: query.Where(p => p.CreatedAt >= dateStart && p.CreatedAt <= dateEnd);

		if (isSuccess.HasValue)
			query = query.Where(p => p.IsSuccess == isSuccess.Value);

		if (responseStatusCode.HasValue)
			query = query.Where(p => p.ResponseStatusCode == responseStatusCode.Value);

		if (!string.IsNullOrEmpty(url?.Trim()))
			query = query.Where(p => p.Url.Contains(url));

		if (!string.IsNullOrWhiteSpace(tenantId))
			query = query.Where(p => p.TenantId == tenantId);

		if (!string.IsNullOrWhiteSpace(correlationId))
			query = query.Where(p => p.CorrelationId == correlationId);

		if (!string.IsNullOrWhiteSpace(traceId))
			query = query.Where(p => p.TraceId == traceId);

		if (pageNumber is not null && pageSize is not null)
			query = query.Skip(PaginationHelper.GetSkip(pageNumber.Value, pageSize.Value)).Take(pageSize.Value);

		return query.OrderBy(o => o.CreatedAt).ToList();
	}
}
