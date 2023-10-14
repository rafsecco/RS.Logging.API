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
app.MapGet("/GetAll/{page:int?}/{pageSize:int?}/", ([FromServices] ILogRepository logRepository, int? page, int? pageSize) =>
{
	var logList = logRepository.GetAll(page, pageSize);
	return logList;
});

app.MapGet("/GetById/{id:int}/", ([FromServices] ILogRepository logRepository, ulong id) =>
{
	var log = logRepository.GetById(id);
	return log;
});

//app.MapGet("/Search/{page:int?}/{pageSize:int?}/", ([FromServices] ILogRepository logRepository, int? page, int? pageSize, SearchLogsViewModel model) =>
//{
//	var logList = logRepository.GetSearch(page, pageSize, model.DateTimeIni, model.DateTimeFim, model.LogLevel, model.Message);
//	return logList;
//});
#endregion

#region Map Post
app.MapPost("/CreateLog", ([FromServices] ILogRepository logRepository, [FromBody] LogsViewModel pModel) =>
{
	var log = new Log(pModel.LogLevel, pModel.Message, pModel.StackTrace);
	var result = logRepository.Create(log);
	return result;
});
#endregion

app.Run();
