using Microsoft.Extensions.Logging;

namespace RS.Logstream.Domain.LogProcess.Contracts;

public interface ILogProcessRepository
{
	ulong CreateLogProcess(LogProcess logProcess);

	bool CreateLogProcessDetail(LogProcessDetail logProcessDetails);

	IEnumerable<LogProcess> GetAll(int? page, int? pageSize, string? tenantId = null);

	LogProcess? GetById(ulong id, string? tenantId = null);

	IEnumerable<LogProcess> Search(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		uint? IdProcess,
		string? processName,
		LogLevel? logLevel,
		string? message,
		string? stackTrace,
		int? pageNumber,
		int? pageSize,
		string? tenantId = null,
		string? correlationId = null,
		string? traceId = null,
		string? fullTextQuery = null
	);

	IEnumerable<LogProcess> GetAudit(
		DateTime? dateTimeStart,
		DateTime? dateTimeEnd,
		ProcessStatus? status,
		int? pageNumber,
		int? pageSize,
		string? tenantId = null
	);
}
