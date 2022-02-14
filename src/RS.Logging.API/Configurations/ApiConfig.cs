using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using RS.Logging.API.Database;
using RS.Logging.API.Domain;

namespace RS.Logging.API.Configurations;

public static class ApiConfig
{
	public static IServiceCollection AddApiConfigureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers();

		string strConn = configuration.GetConnectionString("ConnMariaDB");

		services.AddDbContext<LogsDbContext>
		(
			options => options
				.UseMySql(strConn, ServerVersion.AutoDetect(strConn), e =>
				{
					e.EnableRetryOnFailure(
						maxRetryCount: 3,
						maxRetryDelay: TimeSpan.FromSeconds(5),
						errorNumbersToAdd: null);
				})
				.LogTo(Console.WriteLine)
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors()
		);

		return services;
	}

	public static WebApplication UseApiConfigure(this WebApplication app)
	{
		app.UseHttpsRedirection();
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		app.UseAuthorization();
		app.MapControllers();
		return app;
	}
}
