using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.Log;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.Infra.Contexts;
public class RSLoggingDbContext : DbContext
{
	public DbSet<Log> Logs { get; set; }
	public DbSet<LogProcess> LogProcess { get; set; }
	public DbSet<LogProcessDetail> LogProcessDetails { get; set; }

	public RSLoggingDbContext(DbContextOptions<RSLoggingDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//latin2_general_ci (Default)
		modelBuilder.UseCollation("utf8_general_ci");
		//modelBuilder.HasDefaultSchema("RS.Logging");

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(RSLoggingDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}

}
