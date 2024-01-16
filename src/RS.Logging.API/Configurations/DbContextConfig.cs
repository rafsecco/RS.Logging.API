using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.Log.Contracts;
using RS.Logging.Domain.LogProcess.Contracts;
using RS.Logging.Infra.Contexts;
using RS.Logging.Infra.Repositories;

namespace RS.Logging.API.Configurations;

public static class DbContextConfig
{
	public static WebApplicationBuilder AddDbContextConfig(this WebApplicationBuilder builder)
	{
		string? strConn = builder.Configuration.GetConnectionString("ConnMariaDB");

		builder.Services.AddDbContext<RSLoggingDbContext>(options =>
		{
			options
				.UseMySql(strConn, ServerVersion.AutoDetect(strConn), e =>
				{
					e.EnableRetryOnFailure(
						maxRetryCount: 3,
						maxRetryDelay: TimeSpan.FromSeconds(6),
						errorNumbersToAdd: null);
				})
				.LogTo(Console.WriteLine)
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors();
		});

		builder.Services.AddTransient<ILogRepository, LogRepository>();
		builder.Services.AddTransient<ILogProcessRepository, LogProcessRepository>();

		return builder;
	}
}
