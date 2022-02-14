using Microsoft.OpenApi.Models;

namespace RS.Logging.API.Configurations;

public static class SwaggerConfig
{
	public static IServiceCollection AddSwaggerConfigureServices(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "RS.LogService.API", Version = "v1" });
		});
		return services;
	}

	public static WebApplication UseSwaggerConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RS.LogService.API v1"));
		}
		return app;
	}
}
