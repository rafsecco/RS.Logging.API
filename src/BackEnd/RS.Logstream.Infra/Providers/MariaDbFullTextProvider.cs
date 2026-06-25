using Microsoft.EntityFrameworkCore;
using RS.Logstream.Domain.Log;
using RS.Logstream.Domain.LogProcess;

namespace RS.Logstream.Infra.Providers;

public class MariaDbFullTextProvider : IFullTextSearchProvider
{
	public IQueryable<Log> ApplyToLogs(IQueryable<Log> query, string term) =>
		query.Where(p =>
			EF.Functions.Match(p.Message, term, MySqlMatchSearchMode.NaturalLanguage) > 0 ||
			(p.StackTrace != null && EF.Functions.Match(p.StackTrace, term, MySqlMatchSearchMode.NaturalLanguage) > 0));

	public IQueryable<LogProcessDetail> ApplyToDetails(IQueryable<LogProcessDetail> query, string term) =>
		query.Where(p =>
			EF.Functions.Match(p.Message, term, MySqlMatchSearchMode.NaturalLanguage) > 0 ||
			(p.StackTrace != null && EF.Functions.Match(p.StackTrace, term, MySqlMatchSearchMode.NaturalLanguage) > 0));
}
