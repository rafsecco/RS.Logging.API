using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RS.Logging.API.Configurations;
using RS.Logging.API.Ingestion;
using RS.Logging.API.ViewModels;
using RS.Logging.Domain.Log;
using RS.Logging.Domain.Log.Contracts;
using RS.Logging.Domain.LogProcess;
using RS.Logging.Domain.LogProcess.Contracts;
using System.Diagnostics;
using System;
using RS.Logging.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region ConfigureService
builder
	.AddApiConfig()
	.AddCorsConfig()
	.AddSwaggerConfig()
	.AddDbContextConfig()
	.AddIngestionConfig();
#endregion

var app = builder.Build();

#region Configure
app.UseApiConfigure()
	.UseCorsConfigure()
	.UseSwaggerConfigure();
#endregion


#region Log
app.MapGet("/GetAll/", (
	[FromServices] ILogRepository logRepository,
	[FromQuery(Name = "pn")] int? pageNumber,
	[FromQuery(Name = "ps")] int? pageSize) =>
{
	var logList = logRepository.GetAll(pageNumber, pageSize);
	return Results.Ok(logList);
});

app.MapGet("/GetById/{id:int}", (
	[FromServices] ILogRepository logRepository,
	ulong id) =>
{
	var logList = logRepository.GetById(id);
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

app.MapPost("/CreateLog/", ([FromServices] ILogIngestionQueue ingestionQueue, [FromBody] LogsViewModel pModel) =>
{
	var log = new Log(pModel.LogLevel, pModel.Message, pModel.StackTrace);

	ingestionQueue.Enqueue((services, _) =>
	{
		services.GetRequiredService<ILogRepository>().Create(log);
		return Task.CompletedTask;
	});

	return Results.Accepted();
});
#endregion

#region LogProcess
app.MapGet("/GetAllLogProcess/", (
	[FromServices] ILogProcessRepository logProcessRepository,
	[FromQuery(Name = "pn")] int? pageNumber,
	[FromQuery(Name = "ps")] int? pageSize) =>
{
	var logProcessList = logProcessRepository.GetAll(pageNumber, pageSize);
	return Results.Ok(logProcessList);
});

app.MapGet("/GetLogProcessById/{id:int}/", (
	[FromServices] ILogProcessRepository logProcessRepository,
	ulong id) =>
{
	var logProcess = logProcessRepository.GetById(id);
	return Results.Ok(logProcess);
});

app.MapGet("/LogProcessSearch/", (
	[FromServices] ILogProcessRepository logProcessRepository,
	[FromQuery(Name = "ds")] DateTime? dateStart,
	[FromQuery(Name = "de")] DateTime? dateEnd,
	[FromQuery(Name = "idp")] uint? IdProcess,
	[FromQuery(Name = "nm")] string? processName,
	[FromQuery(Name = "ll")] int? logLevel,
	[FromQuery(Name = "msg")] string? message,
	[FromQuery(Name = "st")] string? stackTrace,
	[FromQuery(Name = "pn")] int? pageNumber,
	[FromQuery(Name = "ps")] int? pageSize) =>
{
	LogLevel? ll = (logLevel is null ? null : (LogLevel)logLevel);

	var logProcessList = logProcessRepository.Search(
		dateStart,
		dateEnd,
		IdProcess,
		processName,
		ll,
		message,
		stackTrace,
		pageNumber,
		pageSize);

	return Results.Ok(logProcessList);
});

app.MapPost("/CreateLogProcess/", (
	[FromServices] ILogProcessRepository logProcessRepository,
	[FromBody] LogProcessViewModel pModel) =>
{
	var logProcess = new LogProcess(pModel.ProcessId, pModel.Name);
	var result = logProcessRepository.CreateLogProcess(logProcess);
	return Results.Ok(result);
});

app.MapPost("/CreateLogProcessDetail/", (
	[FromServices] ILogIngestionQueue ingestionQueue,
	[FromBody] LogProcessDetailsViewModel pModel) =>
{
	var logProcessDetail = new LogProcessDetail(pModel.LogProcessId, pModel.LogLevel, pModel.Message, pModel.StackTrace);

	ingestionQueue.Enqueue((services, _) =>
	{
		services.GetRequiredService<ILogProcessRepository>().CreateLogProcessDetail(logProcessDetail);
		return Task.CompletedTask;
	});

	return Results.Accepted();
});
#endregion

#region Audit
app.MapGet("/LogProcessAudit/", (
	[FromServices] ILogProcessRepository logProcessRepository,
	[FromQuery(Name = "ds")] DateTime? dateStart,
	[FromQuery(Name = "de")] DateTime? dateEnd,
	[FromQuery(Name = "st")] int? status,
	[FromQuery(Name = "pn")] int? pageNumber,
	[FromQuery(Name = "ps")] int? pageSize) =>
{
	ProcessStatus? processStatus = status.HasValue ? (ProcessStatus)status.Value : null;

	var result = logProcessRepository
		.GetAudit(dateStart, dateEnd, processStatus, pageNumber, pageSize)
		.Select(AuditLogProcessViewModel.FromDomain);

	return Results.Ok(result);
});

app.MapGet("/LogProcessAudit/{id:long}", (
	[FromServices] ILogProcessRepository logProcessRepository,
	ulong id) =>
{
	var process = logProcessRepository.GetById(id);

	if (process is null)
		return Results.NotFound();

	return Results.Ok(AuditLogProcessViewModel.FromDomain(process));
});
#endregion

app.Run();
