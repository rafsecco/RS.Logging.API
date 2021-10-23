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

			modelBuilder.Entity<Domain.Log>().HasData(
				new Domain.Log { Id = 1, Message = "teste 1", TenantId = "tenant-1" },
				new Domain.Log { Id = 2, Message = "teste 2", TenantId = "tenant-2" },
				new Domain.Log { Id = 3, Message = "teste 3", TenantId = "tenant-2" }
			);


		}
	}
}
