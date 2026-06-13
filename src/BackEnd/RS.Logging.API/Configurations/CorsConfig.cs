namespace RS.Logging.API.Configurations;

public static class CorsConfig
{
	public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
	{
		var allowedOrigins = builder.Configuration
			.GetSection("Cors:AllowedOrigins")
			.Get<string[]>() ?? [];

		builder.Services.AddCors(options =>
		{
			options.AddPolicy("Development", policy =>
				policy
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader());
			options.AddPolicy("Production", policy =>
				policy
					.WithOrigins(allowedOrigins)
					.WithMethods("POST", "GET")
					.AllowAnyHeader());
		});

		return builder;
	}

	public static WebApplication UseCorsConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			//app.UseCors();
			app.UseCors("Development");
			//app.UseCors("Production");
		}
		else
		{
			app.UseCors("Production");
		}

		return app;
	}
}
