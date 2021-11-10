using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RS.Log.API.Database;
using RS.Log.API.Extensions;
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

			services.AddHttpContextAccessor();
			services.AddScoped<LogsContext>(provider =>
			{
				var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
				var tenantId = httpContext?.GetTenantId();
				var strConn = configuration.GetConnectionString($"Conn{tenantId??"MariaDB"}");

				var optionsBuilder = new DbContextOptionsBuilder<LogsContext>();
				optionsBuilder
					.UseMySql(strConn, ServerVersion.AutoDetect(strConn), p =>
					{
						p.EnableRetryOnFailure(
							maxRetryCount: 3,
							maxRetryDelay: TimeSpan.FromSeconds(5),
							errorNumbersToAdd: null);
					})
					.LogTo(Console.WriteLine)
					.EnableSensitiveDataLogging()
					.EnableDetailedErrors();
				
				return new LogsContext(optionsBuilder.Options);
			});

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

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			return app;
		}
	}

}
