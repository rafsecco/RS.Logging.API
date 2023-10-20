using Microsoft.Extensions.Logging;

namespace RS.Logging.Domain.Log.Contracts;

public interface ILogRepository
{
	bool Create(Log log);

	Log? GetById(ulong id);

	IEnumerable<Log> GetAll();

	IEnumerable<Log> Search(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		LogLevel? logLevel,
		string? message,
		int? pageNumber,
		int? pageSize
	);

}
