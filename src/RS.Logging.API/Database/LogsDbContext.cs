using Microsoft.EntityFrameworkCore;
using RS.Logging.API.Domain;

namespace RS.Logging.API.Database;

public class LogsDbContext : DbContext
{
	public DbSet<Log> Logs { get; set; }

	public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.UseCollation("utf8_general_ci");

		modelBuilder.Entity<Log>().HasKey(k => new { k.Id, k.IdProcess });
		modelBuilder.Entity<Log>().Property(k => k.Id).ValueGeneratedOnAdd();
		modelBuilder.Entity<Log>().Property(k => k.DateCreated).HasDefaultValueSql("NOW()");

		modelBuilder.Entity<Log>().Property(p => p.LogLevel).HasColumnType("SMALLINT");
		modelBuilder.Entity<Log>().Property(p => p.Message).HasColumnType("VARCHAR(255)");

		modelBuilder.Entity<Log>().HasIndex(i => i.DateCreated, "idx_Logs_DateCreated");
		modelBuilder.Entity<Log>().HasIndex(i => new { i.DateCreated, i.IdProcess }, "idx_Logs_DateCreated-IdProcess");
		modelBuilder.Entity<Log>().HasIndex(i => new { i.DateCreated, i.LogLevel }, "idx_Logs_DateCreated-LogLevel");
	}
}
