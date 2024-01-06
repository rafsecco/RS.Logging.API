namespace RS.Logging.Domain.LogProcess.Contracts;

public interface ILogProcessRepository
{
	ulong CreateLogProcess(LogProcess logProcess);

	LogProcess? GetById(ulong id);

	IEnumerable<LogProcess> GetAll(int? page, int? pageSize);

	//IEnumerable<LogProcess> Search(
	//	DateTime? dateTimeStart,
	//	DateTime? dateTimeEnd,
	//	LogLevel? logLevel,
	//	string? message,
	//	int? pageNumber,
	//	int? pageSize
	//);

	bool CreateLogProcessDetail(LogProcessDetail logProcessDetails);

}
