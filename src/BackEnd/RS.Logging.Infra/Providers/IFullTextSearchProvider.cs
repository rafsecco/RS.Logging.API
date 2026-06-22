using RS.Logging.Domain.Log;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.Infra.Providers;

public interface IFullTextSearchProvider
{
    IQueryable<Log> ApplyToLogs(IQueryable<Log> query, string term);
    IQueryable<LogProcessDetail> ApplyToDetails(IQueryable<LogProcessDetail> query, string term);
}
