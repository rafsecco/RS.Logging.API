using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RS.Logstream.API.Configurations;

public static class AuthConfig
{
	public static WebApplicationBuilder AddAuthConfig(this WebApplicationBuilder builder)
	{
		var authSection = builder.Configuration.GetSection("Auth");

		builder.Services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority            = authSection["Authority"];
				options.Audience             = authSection["Audience"];
				options.RequireHttpsMetadata = authSection.GetValue<bool>("RequireHttpsMetadata");

				options.TokenValidationParameters = new()
				{
					ValidateIssuer           = true,
					ValidIssuer              = authSection["Issuer"],
					ValidateAudience         = true,
					ValidateLifetime         = true,
					ValidateIssuerSigningKey = true,
					ClockSkew                = TimeSpan.FromSeconds(30)
				};

				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = ctx =>
					{
						ctx.HttpContext.RequestServices
							.GetRequiredService<ILogger<JwtBearerEvents>>()
							.LogWarning("JWT inválido: {Error}", ctx.Exception.Message);
						return Task.CompletedTask;
					}
				};
			});

		builder.Services.AddAuthorization();
		return builder;
	}
}
