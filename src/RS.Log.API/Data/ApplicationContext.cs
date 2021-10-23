using Microsoft.EntityFrameworkCore;
using RS.Log.API.Provider;

namespace RS.Log.API.Data
{
	public class ApplicationContext : DbContext
	{
		private readonly TenantData _tenant;
		public DbSet<Domain.Log> Logs { get; set; }

		public ApplicationContext(
			DbContextOptions<ApplicationContext> options,
			TenantData tenant) : base(options)
		{
			_tenant = tenant;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Domain.Log>()
				.HasQueryFilter(p => p.TenantId == _tenant.TenantId)
				.HasIndex(idx => new { idx.DateCreated, idx.TenantId })
				.HasDatabaseName("idx_tenantid");
		}
	}
}
