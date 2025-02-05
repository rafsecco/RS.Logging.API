using Microsoft.Extensions.Logging;

namespace RS.Logging.Domain.LogProcess.Contracts;

public interface ILogProcessRepository
{
	ulong CreateLogProcess(LogProcess logProcess);

	bool CreateLogProcessDetail(LogProcessDetail logProcessDetails);

	IEnumerable<LogProcess> GetAll(int? page, int? pageSize);

	LogProcess? GetById(ulong id);

	IEnumerable<LogProcess> Search(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		uint? IdProcess,
		string? processName,
		LogLevel? logLevel,
		string? message,
		string? stackTrace,
		int? pageNumber,
		int? pageSize
	);
}
