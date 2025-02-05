using Microsoft.OpenApi.Models;

namespace RS.Logging.API.Configurations;

public static class SwaggerConfig
{

	public static WebApplicationBuilder AddSwaggerConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "RS.Logging.API", Version = "v1" });
		});

		//builder.Services.AddSwaggerGen(c =>
		//{
		//	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		//	{
		//		Description = "Insira o token JWT desta maneira: Bearer {seu token}",
		//		Name = "Authorization",
		//		Scheme = "Bearer",
		//		BearerFormat = "JWT",
		//		In = ParameterLocation.Header,
		//		Type = SecuritySchemeType.ApiKey
		//	});
		//	c.AddSecurityRequirement(new OpenApiSecurityRequirement
		//	{
		//		{
		//			new OpenApiSecurityScheme {
		//				Reference = new OpenApiReference {
		//					Type = ReferenceType.SecurityScheme,
		//					Id = "Bearer"
		//				}
		//			},
		//			new string[] {}
		//		}

		//	});
		//});
		return builder;
	}

	public static WebApplication UseSwaggerConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RS.Logging.API v1"));
		}
		return app;
	}
}
