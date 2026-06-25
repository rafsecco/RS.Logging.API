using RS.Logstream.Domain.Log;
using RS.Logstream.Domain.LogProcess;

namespace RS.Logstream.Infra.Providers;

public interface IFullTextSearchProvider
{
	IQueryable<Log> ApplyToLogs(IQueryable<Log> query, string term);
	IQueryable<LogProcessDetail> ApplyToDetails(IQueryable<LogProcessDetail> query, string term);
}
