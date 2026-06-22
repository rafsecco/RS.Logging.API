using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.ApiCall;
using RS.Logging.Domain.Log;
using RS.Logging.Domain.LogProcess;
using RS.Logging.Infra.Mappings;
using RS.Logging.Infra.Providers;

namespace RS.Logging.Infra.Contexts;
public class RSLoggingDbContext : DbContext
{
	private readonly IDbColumnTypes _columnTypes;

	public DbSet<Log> Logs { get; set; }
	public DbSet<LogProcess> LogProcess { get; set; }
	public DbSet<LogProcessDetail> LogProcessDetails { get; set; }
	public DbSet<ApiCallLog> ApiCallLogs { get; set; }

	public RSLoggingDbContext(DbContextOptions<RSLoggingDbContext> options, IDbColumnTypes columnTypes)
		: base(options)
	{
		_columnTypes = columnTypes;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		if (Database.ProviderName?.Contains("MySql") == true)
			modelBuilder.UseCollation("utf8_general_ci");

		modelBuilder.ApplyConfiguration(new LogMappings(_columnTypes));
		modelBuilder.ApplyConfiguration(new LogProcessMappings(_columnTypes));
		modelBuilder.ApplyConfiguration(new LogProcessDetailsMappings(_columnTypes));
		modelBuilder.ApplyConfiguration(new ApiCallLogMappings(_columnTypes));

		base.OnModelCreating(modelBuilder);
	}
}
