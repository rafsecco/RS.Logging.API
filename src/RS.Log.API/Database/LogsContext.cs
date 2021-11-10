using System;
using Microsoft.EntityFrameworkCore;

namespace RS.Log.API.Database
{
	public class LogsContext : DbContext
	{
		public DbSet<Domain.Log> Logs { get; set; }

		public LogsContext(DbContextOptions<LogsContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.UseCollation("utf8_general_ci");
			modelBuilder.Entity<Domain.Log>().HasIndex(i => i.DateCreated, "idx_Logs_DateCreated");
			modelBuilder.Entity<Domain.Log>().HasIndex(i => new { i.DateCreated, i.Message }, "idx_Logs_DateCreated-Message");
		}
	}
}
