using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RS.Logstream.API.Ingestion;
using RS.Logstream.API.ViewModels;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Domain.LogProcess.Contracts;

namespace RS.Logstream.API.Endpoints;

public static class LogProcessEndpoints
{
	public static WebApplication MapLogProcessEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/log-process").RequireAuthorization();

		group.MapGet("/", (
			[FromServices] ILogProcessRepository logProcessRepository,
			[FromQuery(Name = "pn")] int? pageNumber,
			[FromQuery(Name = "ps")] int? pageSize,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			var logProcessList = logProcessRepository.GetAll(pageNumber, pageSize, tenantId);
			return Results.Ok(logProcessList);
		});

		group.MapGet("/{id}", (
			[FromServices] ILogProcessRepository logProcessRepository,
			long id,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			var logProcess = logProcessRepository.GetById(id, tenantId);
			return Results.Ok(logProcess);
		});

		group.MapGet("/search", (
			[FromServices] ILogProcessRepository logProcessRepository,
			[FromQuery(Name = "ds")] DateTime? dateStart,
			[FromQuery(Name = "de")] DateTime? dateEnd,
			[FromQuery(Name = "idp")] uint? IdProcess,
			[FromQuery(Name = "nm")] string? processName,
			[FromQuery(Name = "ll")] int? logLevel,
			[FromQuery(Name = "msg")] string? message,
			[FromQuery(Name = "st")] string? stackTrace,
			[FromQuery(Name = "pn")] int? pageNumber,
			[FromQuery(Name = "ps")] int? pageSize,
			[FromQuery(Name = "q")] string? fullTextQuery,
			[FromQuery(Name = "cid")] string? correlationId,
			[FromQuery(Name = "tid")] string? traceId,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			LogLevel? ll = logLevel is null ? null : (LogLevel)logLevel;
			var logProcessList = logProcessRepository.Search(
				dateStart, dateEnd, IdProcess, processName, ll, message, stackTrace,
				pageNumber, pageSize, tenantId, correlationId, traceId, fullTextQuery);
			return Results.Ok(logProcessList);
		});

		group.MapPost("/", (
			[FromServices] ILogProcessRepository logProcessRepository,
			[FromBody] LogProcessViewModel pModel,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId,
			[FromHeader(Name = "X-Correlation-Id")] string? correlationId,
			[FromHeader(Name = "X-Trace-Id")] string? traceId) =>
		{
			var logProcess = new LogProcess(pModel.ProcessId, pModel.Name, null, tenantId, correlationId, traceId);
			var result = logProcessRepository.CreateLogProcess(logProcess);
			return Results.Ok(result);
		});

		group.MapPost("/detail", (
			[FromServices] ILogIngestionQueue ingestionQueue,
			[FromBody] LogProcessDetailsViewModel pModel,
			[FromHeader(Name = "X-Correlation-Id")] string? correlationId,
			[FromHeader(Name = "X-Trace-Id")] string? traceId) =>
		{
			var detail = new LogProcessDetail(pModel.LogProcessId, pModel.LogLevel, pModel.Message, pModel.StackTrace, correlationId, traceId);
			ingestionQueue.Enqueue((services, _) =>
			{
				services.GetRequiredService<ILogProcessRepository>().CreateLogProcessDetail(detail);
				return Task.CompletedTask;
			});
			return Results.Accepted();
		});

		group.MapPost("/detail/batch", (
			[FromServices] ILogIngestionQueue ingestionQueue,
			[FromBody] List<LogProcessDetailsViewModel> pModels,
			[FromHeader(Name = "X-Correlation-Id")] string? correlationId,
			[FromHeader(Name = "X-Trace-Id")] string? traceId) =>
		{
			foreach (var pModel in pModels)
			{
				var detail = new LogProcessDetail(pModel.LogProcessId, pModel.LogLevel, pModel.Message, pModel.StackTrace, correlationId, traceId);
				ingestionQueue.Enqueue((services, _) =>
				{
					services.GetRequiredService<ILogProcessRepository>().CreateLogProcessDetail(detail);
					return Task.CompletedTask;
				});
			}
			return Results.Accepted();
		});

		return app;
	}
}
