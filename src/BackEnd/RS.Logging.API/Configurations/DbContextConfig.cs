using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.ApiCall.Contracts;
using RS.Logging.Domain.Log.Contracts;
using RS.Logging.Domain.LogProcess.Contracts;
using RS.Logging.Infra.Contexts;
using RS.Logging.Infra.Providers;
using RS.Logging.Infra.Repositories;

namespace RS.Logging.API.Configurations;

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
                             .MigrationsAssembly("RS.Logging.Infra")));
                break;

            case "Postgres":
                builder.Services.AddSingleton<IDbColumnTypes, PostgresColumnTypes>();
                builder.Services.AddSingleton<IFullTextSearchProvider, LikeFullTextProvider>();
                builder.Services.AddDbContext<RSLoggingDbContext>(opt =>
                    opt.UseNpgsql(
                        builder.Configuration.GetConnectionString("ConnPostgres"),
                        o => o.EnableRetryOnFailure(3, TimeSpan.FromSeconds(6), null)
                             .MigrationsHistoryTable("__EFMigrationsHistory_Postgres")
                             .MigrationsAssembly("RS.Logging.Infra")));
                break;

            default: // MariaDB
                var strConn = builder.Configuration.GetConnectionString("ConnMariaDB");
                builder.Services.AddSingleton<IDbColumnTypes, MariaDbColumnTypes>();
                builder.Services.AddSingleton<IFullTextSearchProvider, MariaDbFullTextProvider>();
                builder.Services.AddDbContext<RSLoggingDbContext>(opt =>
                    opt.UseMySql(strConn, ServerVersion.AutoDetect(strConn),
                        o => o.EnableRetryOnFailure(3, TimeSpan.FromSeconds(6), null)
                             .MigrationsAssembly("RS.Logging.Infra"))
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
