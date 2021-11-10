using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RS.Log.API.Configurations;
using RS.Log.API.Database;

namespace RS.Log.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApiConfigureServices(Configuration);
			services.AddSwaggerConfigureServices();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseApiConfigure(env);
			app.UseSwaggerConfigure(env);

			//DatabaseInitialize(app);
		}

		private void DatabaseInitialize(IApplicationBuilder app)
		{
			using var db = app.ApplicationServices
				.CreateScope()
				.ServiceProvider
				.GetRequiredService<LogsContext>();

			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();

			db.SaveChanges();
		}

	}
}
