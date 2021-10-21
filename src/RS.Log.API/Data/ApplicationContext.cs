using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Log.API.Data
{
	public class ApplicationContext : DbContext
	{
		public DbSet<Domain.Log> Logs { get; set; }

		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{

		}
	}
}
