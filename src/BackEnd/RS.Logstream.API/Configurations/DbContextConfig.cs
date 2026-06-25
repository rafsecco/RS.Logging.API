using Microsoft.EntityFrameworkCore;
using RS.Logstream.Domain.ApiCall.Contracts;
using RS.Logstream.Domain.Log.Contracts;
using RS.Logstream.Domain.LogProcess.Contracts;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Providers;
using RS.Logstream.Infra.Repositories;

namespace RS.Logstream.API.Configurations;

public static class DbContextConfig
{
	public static WebApplicationBuilder AddDbContextConfig(this WebApplicationBuilder builder)
	{
		var provider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "MariaDB";

		switch (provider)
		{
			case "SqlServer":
				builder.Services.AddSingleton<IDbColumnTypes, SqlServerColumnTypes>();
				builder.Services.AddSingleton<IFullTextSearchProvider, LikeFullTextProvider>();
				builder.Services.AddDbContext<RSLoggingDbContext>(opt =>
					opt.UseSqlServer(
						builder.Configuration.GetConnectionString("ConnSqlServer"),
						o => o.EnableRetryOnFailure(3, TimeSpan.FromSeconds(6), null)
							 .MigrationsHistoryTable("__EFMigrationsHistory_SqlServer")
							 .MigrationsAssembly("RS.Logstream.Infra")));
				break;

			case "Postgres":
				builder.Services.AddSingleton<IDbColumnTypes, PostgresColumnTypes>();
				builder.Services.AddSingleton<IFullTextSearchProvider, LikeFullTextProvider>();
				builder.Services.AddDbContext<RSLoggingDbContext>(opt =>
					opt.UseNpgsql(
						builder.Configuration.GetConnectionString("ConnPostgres"),
						o => o.EnableRetryOnFailure(3, TimeSpan.FromSeconds(6), null)
							 .MigrationsHistoryTable("__EFMigrationsHistory_Postgres")
							 .MigrationsAssembly("RS.Logstream.Infra")));
				break;

			default: // MariaDB
				var strConn = builder.Configuration.GetConnectionString("ConnMariaDB");
				builder.Services.AddSingleton<IDbColumnTypes, MariaDbColumnTypes>();
				builder.Services.AddSingleton<IFullTextSearchProvider, MariaDbFullTextProvider>();
				builder.Services.AddDbContext<RSLoggingDbContext>(opt =>
					opt.UseMySql(strConn, ServerVersion.AutoDetect(strConn),
						o => o.EnableRetryOnFailure(3, TimeSpan.FromSeconds(6), null)
							 .MigrationsAssembly("RS.Logstream.Infra"))
					.LogTo(Console.WriteLine)
					.EnableSensitiveDataLogging()
					.EnableDetailedErrors());
				break;
		}

		builder.Services.AddTransient<ILogRepository, LogRepository>();
		builder.Services.AddTransient<ILogProcessRepository, LogProcessRepository>();
		builder.Services.AddTransient<IApiCallLogRepository, ApiCallLogRepository>();

		return builder;
	}
}
