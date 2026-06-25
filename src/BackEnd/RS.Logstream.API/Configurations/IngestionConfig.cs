using RS.Logstream.API.Ingestion;

namespace RS.Logstream.API.Configurations;

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
