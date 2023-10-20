using Microsoft.AspNetCore.Mvc;
using RS.Logging.API.Configurations;
using RS.Logging.API.ViewModels;
using RS.Logging.Domain.Log;
using RS.Logging.Domain.Log.Contracts;

var builder = WebApplication.CreateBuilder(args);

#region ConfigureService
builder.Services.AddApiConfigureServices(builder.Configuration);
builder.Services.AddSwaggerConfigureServices();
#endregion

var app = builder.Build();

#region Configure
app.UseApiConfigure();
app.UseSwaggerConfigure();
#endregion

#region Map Get
app.MapGet("/GetAll/", ([FromServices] ILogRepository logRepository) =>
{
	var logList = logRepository.GetAll();
	return Results.Ok(logList);
});

app.MapGet("/Search/", (
	[FromServices] ILogRepository logRepository,
	[FromQuery(Name = "ds")] DateTime? dateStart,
	[FromQuery(Name = "de")] DateTime? dateEnd,
	[FromQuery(Name = "ll")] int? logLevel,
	[FromQuery(Name = "msg")] string? message,
	[FromQuery(Name = "pn")] int? pageNumber,
	[FromQuery(Name = "ps")] int? pageSize) =>
{
	LogLevel? ll = (logLevel is null ? null : (LogLevel)logLevel);

	var logList = logRepository.Search(dateStart, dateEnd, ll, message, pageNumber, pageSize);
	return Results.Ok(logList);
});

// Exemplo de chamada por rota e query
//app.MapGet("/Search/{message?}", (
//	[FromServices] ILogRepository logRepository,
//	[FromRoute] string? message,
//	[FromQuery(Name = "ds")] DateTime? dateStart,
//	[FromQuery(Name = "de")] DateTime? dateEnd,
//	[FromQuery(Name = "p")] int? page,
//	[FromQuery(Name = "ps")] int? pageSize) =>
//{
//	var logList = logRepository.Search(dateStart, dateEnd, message, page, pageSize);
//	return Results.Ok(logList);
//});
#endregion

#region Map Post
app.MapPost("/CreateLog/", ([FromServices] ILogRepository logRepository, [FromBody] LogsViewModel pModel) =>
{
	var log = new Log(pModel.LogLevel, pModel.Message, pModel.StackTrace);
	var result = logRepository.Create(log);
	return Results.Ok(result);
});
#endregion

app.Run();
