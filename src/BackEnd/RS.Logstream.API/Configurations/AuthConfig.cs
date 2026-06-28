using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RS.Logstream.API.Configurations;

public static class AuthConfig
{
	public static WebApplicationBuilder AddAuthConfig(this WebApplicationBuilder builder)
	{
		var authSection = builder.Configuration.GetSection("Auth");
		var authority   = authSection["Authority"];
		var secret      = authSection["Secret"];

		if (string.IsNullOrEmpty(authority) && string.IsNullOrEmpty(secret))
			throw new InvalidOperationException(
				"Auth: configure Auth:Authority (produção) ou Auth:Secret (desenvolvimento).");

		if (!string.IsNullOrEmpty(secret) && !builder.Environment.IsDevelopment())
			throw new InvalidOperationException(
				"Auth:Secret não pode ser usado fora de Development. Configure Auth:Authority.");

		builder.Services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = authSection.GetValue<bool>("RequireHttpsMetadata");

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer           = true,
					ValidIssuer              = authSection["Issuer"],
					ValidateAudience         = true,
					ValidAudience            = authSection["Audience"],
					ValidateLifetime         = true,
					ValidateIssuerSigningKey = true,
					ClockSkew                = TimeSpan.FromSeconds(30)
				};

				if (!string.IsNullOrEmpty(authority))
				{
					// OIDC mode: signing keys fetched from Authority discovery endpoint
					options.Authority = authority;
					options.Audience  = authSection["Audience"];
				}
				else
				{
					// Symmetric key mode: dev/Docker without an external auth server
					options.TokenValidationParameters.IssuerSigningKey =
						new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
				}

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
