using RS.Logging.API.Configurations;
using RS.Logging.API.Endpoints;

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

app.Run();
