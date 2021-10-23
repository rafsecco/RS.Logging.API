using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RS.Log.API.Data;
using RS.Log.API.Middlewares;
using RS.Log.API.Provider;
using System;

namespace RS.Log.API.Configurations
{
	public static class ApiConfig
	{
		public static IServiceCollection AddApiConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<TenantData>();

			services.AddControllers();

			var strConn = configuration.GetConnectionString("MariaDBConn");
			services.AddDbContext<ApplicationContext>(p => p
				.UseMySql(strConn, ServerVersion.AutoDetect(strConn))
				.LogTo(Console.WriteLine)
				.EnableSensitiveDataLogging()
			);

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

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseMiddleware<TenantMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			return app;
		}
	}
}
