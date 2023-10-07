namespace RS.Logging.Domain.Log.Contracts;

public interface ILogRepository
{
	void Create(Log log);

	Log? GetById(ulong id);

	IEnumerable<Log> GetAll();
}
