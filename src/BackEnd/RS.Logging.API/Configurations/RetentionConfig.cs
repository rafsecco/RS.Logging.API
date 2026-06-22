using RS.Logging.API.Retention;

namespace RS.Logging.API.Configurations;

public static class RetentionConfig
{
	public static WebApplicationBuilder AddRetentionConfig(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<RetentionOptions>(builder.Configuration.GetSection("Retention"));
		builder.Services.AddHostedService<LogRetentionWorker>();

		return builder;
	}
}
