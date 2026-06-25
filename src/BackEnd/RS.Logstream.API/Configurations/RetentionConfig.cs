using RS.Logstream.API.Retention;

namespace RS.Logstream.API.Configurations;

public static class RetentionConfig
{
	public static WebApplicationBuilder AddRetentionConfig(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<RetentionOptions>(builder.Configuration.GetSection("Retention"));
		builder.Services.AddHostedService<LogRetentionWorker>();

		return builder;
	}
}
