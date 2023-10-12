namespace RS.Logging.Domain.Log.Contracts;

public interface ILogRepository
{
	bool Create(Log log);

	Log? GetById(ulong id);

	IEnumerable<Log> GetAll();
}
