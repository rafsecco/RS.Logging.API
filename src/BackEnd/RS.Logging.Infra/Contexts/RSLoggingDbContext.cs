using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.ApiCall;
using RS.Logging.Domain.Log;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.Infra.Contexts;
public class RSLoggingDbContext : DbContext
{
	public DbSet<Log> Logs { get; set; }
	public DbSet<LogProcess> LogProcess { get; set; }
	public DbSet<LogProcessDetail> LogProcessDetails { get; set; }
	public DbSet<ApiCallLog> ApiCallLogs { get; set; }

	public RSLoggingDbContext(DbContextOptions<RSLoggingDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
			modelBuilder.UseCollation("utf8_general_ci");

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(RSLoggingDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}

}
