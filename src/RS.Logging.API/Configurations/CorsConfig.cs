namespace RS.Logging.API.Configurations;

public static class CorsConfig
{
	public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddCors(options =>
		{
			options.AddPolicy("Development", builder =>
				builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader());
			//options.AddPolicy("Production", builder =>
			//	builder
			//		.WithOrigins("https://localhost:7000")
			//		.WithMethods("POST")
			//		.AllowAnyHeader());
		});

		return builder;
	}

	public static WebApplication UseCorsConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseCors();
			//app.UseCors("Development");
			//app.UseCors("Production");
		}
		else
		{
			app.UseCors("Production");
		}

		return app;
	}
}
