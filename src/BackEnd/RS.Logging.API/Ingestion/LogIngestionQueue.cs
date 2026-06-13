using System.Threading.Channels;

namespace RS.Logging.API.Ingestion;

public class LogIngestionQueue : ILogIngestionQueue
{
	private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _channel =
		Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

	public ChannelReader<Func<IServiceProvider, CancellationToken, Task>> Reader => _channel.Reader;

	public void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem)
	{
		ArgumentNullException.ThrowIfNull(workItem);
		_channel.Writer.TryWrite(workItem);
	}
}
