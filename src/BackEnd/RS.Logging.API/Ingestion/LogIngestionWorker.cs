namespace RS.Logging.API.Ingestion;

public class LogIngestionWorker : BackgroundService
{
	private readonly LogIngestionQueue _queue;
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<LogIngestionWorker> _logger;

	public LogIngestionWorker(LogIngestionQueue queue, IServiceProvider serviceProvider, ILogger<LogIngestionWorker> logger)
	{
		_queue = queue;
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await foreach (var workItem in _queue.Reader.ReadAllAsync(stoppingToken))
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				await workItem(scope.ServiceProvider, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Falha ao processar item da fila de ingestão de logs.");
			}
		}
	}
}
