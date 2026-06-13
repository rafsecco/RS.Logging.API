using RS.Logging.API.Ingestion;

namespace RS.Logging.API.Configurations;

public static class IngestionConfig
{
	public static WebApplicationBuilder AddIngestionConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddSingleton<LogIngestionQueue>();
		builder.Services.AddSingleton<ILogIngestionQueue>(sp => sp.GetRequiredService<LogIngestionQueue>());
		builder.Services.AddHostedService<LogIngestionWorker>();

		return builder;
	}
}
