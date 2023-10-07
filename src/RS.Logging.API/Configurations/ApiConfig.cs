using Microsoft.EntityFrameworkCore;
using RS.Logging.Infra.Contexts;

namespace RS.Logging.API.Configurations;

public static class ApiConfig
{
	public static IServiceCollection AddApiConfigureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers();

		string? strConn = configuration.GetConnectionString("ConnMariaDB");

		services.AddDbContext<RSLoggingDbContext>
		(
			options => options
				.UseMySql(strConn, ServerVersion.AutoDetect(strConn), e =>
				{
					e.EnableRetryOnFailure(
						maxRetryCount: 3,
						maxRetryDelay: TimeSpan.FromSeconds(6),
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

		app.UseCors(x => x
			.AllowAnyOrigin()
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());

		app.UseAuthorization();
		app.MapControllers();
		return app;
	}
}
