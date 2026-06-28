using Microsoft.EntityFrameworkCore;
using RS.Logstream.Domain.ApiCall;
using RS.Logstream.Domain.Log;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Infra.Mappings;
using RS.Logstream.Infra.Providers;

namespace RS.Logstream.Infra.Contexts;
public class RSLogstreamDbContext : DbContext
{
	private readonly IDbColumnTypes _columnTypes;

	public DbSet<Log> Logs { get; set; }
	public DbSet<LogProcess> LogProcess { get; set; }
	public DbSet<LogProcessDetail> LogProcessDetails { get; set; }
	public DbSet<ApiCallLog> ApiCallLogs { get; set; }

	public RSLogstreamDbContext(DbContextOptions<RSLogstreamDbContext> options, IDbColumnTypes columnTypes)
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
