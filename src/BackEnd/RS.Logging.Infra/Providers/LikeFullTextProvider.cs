using RS.Logging.Domain.Log;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.Infra.Providers;

public class LikeFullTextProvider : IFullTextSearchProvider
{
    public IQueryable<Log> ApplyToLogs(IQueryable<Log> query, string term) =>
        query.Where(p =>
            p.Message.Contains(term) ||
            (p.StackTrace != null && p.StackTrace.Contains(term)));

    public IQueryable<LogProcessDetail> ApplyToDetails(IQueryable<LogProcessDetail> query, string term) =>
        query.Where(p =>
            p.Message.Contains(term) ||
            (p.StackTrace != null && p.StackTrace.Contains(term)));
}
