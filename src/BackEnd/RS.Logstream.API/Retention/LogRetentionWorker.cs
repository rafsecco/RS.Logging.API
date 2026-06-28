using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RS.Logstream.Infra.Contexts;

namespace RS.Logstream.API.Retention;

public class LogRetentionWorker : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IOptionsMonitor<RetentionOptions> _options;
	private readonly ILogger<LogRetentionWorker> _logger;

	public LogRetentionWorker(IServiceProvider serviceProvider, IOptionsMonitor<RetentionOptions> options, ILogger<LogRetentionWorker> logger)
	{
		_serviceProvider = serviceProvider;
		_options = options;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await PurgeExpiredLogsAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Falha ao executar o expurgo de retenção de logs.");
			}

			var interval = TimeSpan.FromHours(_options.CurrentValue.IntervalHours);
			await Task.Delay(interval, stoppingToken);
		}
	}

	private async Task PurgeExpiredLogsAsync(CancellationToken cancellationToken)
	{
		var options = _options.CurrentValue;

		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<RSLogstreamDbContext>();

		var tenantIds = await context.Logs
			.Select(l => l.TenantId)
			.Union(context.LogProcess.Select(p => p.TenantId))
			.ToListAsync(cancellationToken);

		foreach (var tenantId in tenantIds)
		{
			try
			{
				var days = !string.IsNullOrEmpty(tenantId) && options.PolicyByTenant.TryGetValue(tenantId, out var tenantDays)
					? tenantDays
					: options.DefaultDays;

				var cutoff = DateTime.Now.AddDays(-days);

				await context.Logs
					.Where(l => l.TenantId == tenantId && l.CreatedAt < cutoff)
					.ExecuteDeleteAsync(cancellationToken);

				await context.LogProcess
					.Where(p => p.TenantId == tenantId && p.CreatedAt < cutoff)
					.ExecuteDeleteAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Falha ao expurgar logs do tenant '{TenantId}'.", tenantId ?? "(sem tenant)");
			}
		}
	}
}
