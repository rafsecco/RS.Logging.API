using Microsoft.EntityFrameworkCore;
using RS.Logstream.API.Configurations;
using RS.Logstream.API.Endpoints;
using RS.Logstream.Infra.Contexts;
using RS.Logstream.Infra.Seed;

var builder = WebApplication.CreateBuilder(args);

builder
	.AddApiConfig()
	.AddCorsConfig()
	.AddSwaggerConfig()
	.AddDbContextConfig()
	.AddIngestionConfig()
	.AddRetentionConfig()
	.AddAuthConfig();

var app = builder.Build();

app.UseApiConfigure()
   .UseCorsConfigure()
   .UseSwaggerConfigure();

app.MapLogEndpoints()
   .MapLogProcessEndpoints()
   .MapAuditEndpoints()
   .MapApiCallLogEndpoints();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<RSLoggingDbContext>();
	context.Database.Migrate();

	if (app.Environment.IsDevelopment())
		new DataSeeder(context).Seed();
}

app.Run();
