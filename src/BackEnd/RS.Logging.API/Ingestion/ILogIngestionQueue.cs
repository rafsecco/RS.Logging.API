namespace RS.Logging.API.Ingestion;

public interface ILogIngestionQueue
{
	void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem);
}
