using Microsoft.Extensions.Logging;

namespace RS.Logging.Domain.Log.Contracts;

public interface ILogRepository
{
	bool Create(Log log);

	Log GetById(ulong id);

	IEnumerable<Log> GetAll(int? page = null, int? pageSize = null);

	IEnumerable<Log> GetSearch(
		int? page = null,
		int? pageSize = null,
		DateTime? DateTimeIni = null,
		DateTime? DateTimeFim = null,
		LogLevel? LogLevel = null,
		string? Message = null
	);
}
