using Microsoft.AspNetCore.HttpLogging;
using System.Text.Json.Serialization;

namespace RS.Logging.API.Configurations;

public static class ApiConfig
{
	public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
			options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

		builder.Services.AddControllers();

		builder.Services.AddHttpLogging(options =>
		{
			options.LoggingFields = HttpLoggingFields.RequestMethod
				| HttpLoggingFields.RequestPath
				| HttpLoggingFields.RequestQuery
				| HttpLoggingFields.ResponseStatusCode
				| HttpLoggingFields.Duration;
		});

		return builder;
	}

	public static WebApplication UseApiConfigure(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
			app.UseDeveloperExceptionPage();

		app.UseHttpLogging();
		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		return app;
	}
}
