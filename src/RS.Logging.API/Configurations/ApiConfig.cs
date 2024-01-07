using Microsoft.EntityFrameworkCore;
using RS.Logging.Domain.Log.Contracts;
using RS.Logging.Domain.LogProcess.Contracts;
using RS.Logging.Infra.Contexts;
using RS.Logging.Infra.Repositories;

namespace RS.Logging.API.Configurations;

public static class ApiConfig
{
	public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddControllers();
			//.ConfigureApiBehaviorOptions(options =>
			//{
			//	options.SuppressModelStateInvalidFilter = true;
			//});

		return builder;
	}

	public static WebApplication UseApiConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();

		return app;
	}


	//public static IServiceCollection AddApiConfigureServices(this IServiceCollection services, IConfiguration configuration)
	//{
	//	services.AddControllers();

	//	string? strConn = configuration.GetConnectionString("ConnMariaDB");

	//	services.AddDbContext<RSLoggingDbContext>
	//	(
	//		options => options
	//			.UseMySql(strConn, ServerVersion.AutoDetect(strConn), e =>
	//			{
	//				e.EnableRetryOnFailure(
	//					maxRetryCount: 3,
	//					maxRetryDelay: TimeSpan.FromSeconds(6),
	//					errorNumbersToAdd: null);
	//			})
	//			.LogTo(Console.WriteLine)
	//			.EnableSensitiveDataLogging()
	//			.EnableDetailedErrors()
	//	);

	//	services.AddTransient<ILogRepository, LogRepository>();
	//	services.AddTransient<ILogProcessRepository, LogProcessRepository>();

	//	return services;
	//}

	//public static WebApplication UseApiConfigure(this WebApplication app)
	//{
	//	app.UseHttpsRedirection();
	//	if (app.Environment.IsDevelopment())
	//	{
	//		app.UseDeveloperExceptionPage();
	//	}

	//	app.UseCors(x => x
	//		.AllowAnyOrigin()
	//		.AllowAnyMethod()
	//		.AllowAnyHeader());

	//	app.UseAuthorization();
	//	app.MapControllers();
	//	return app;
	//}
}
