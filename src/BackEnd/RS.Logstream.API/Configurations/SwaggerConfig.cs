using Microsoft.OpenApi.Models;

namespace RS.Logstream.API.Configurations;

public static class SwaggerConfig
{
	public static WebApplicationBuilder AddSwaggerConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "RS.Logstream", Version = "v1" });

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Type         = SecuritySchemeType.Http,
				Scheme       = "bearer",
				BearerFormat = "JWT",
				Description  = "Informe o JWT emitido pela Auth API"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id   = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});

		return builder;
	}

	public static WebApplication UseSwaggerConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RS.Logstream v1"));
		}
		return app;
	}
}
