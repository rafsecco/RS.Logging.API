using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RS.Log.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Log.API.Configurations
{
	public static class ApiConfig
	{
		public static IServiceCollection AddApiConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			var strConn = configuration.GetConnectionString("MariaDBConn");
			services.AddDbContext<ApplicationContext>(p => p
				.UseMySql(strConn, ServerVersion.AutoDetect(strConn))
				.LogTo(Console.WriteLine)
				.EnableSensitiveDataLogging()
			);

			services.AddControllers();

			return services;
		}

		public static IApplicationBuilder UseApiConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RS.Log.API v1"));
			}

			DatabaseInitialize(app);

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			return app;
		}

		private static void DatabaseInitialize(IApplicationBuilder app)
		{
			using var db = app.ApplicationServices
				.CreateScope()
				.ServiceProvider
				.GetRequiredService<ApplicationContext>();

			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();

			for (var i = 1; i <= 5; i++)
			{
				db.Logs.Add(new Domain.Log
				{
					Project = $"Projetct {i}",
					Message = $"Message {i}",
					StackTrace = $"StackTrace {i}"
				});
			}

			db.SaveChanges();
		}
	}
}
