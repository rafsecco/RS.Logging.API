using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RS.Logstream.API.Ingestion;
using RS.Logstream.API.ViewModels;
using RS.Logstream.Domain.Log;
using RS.Logstream.Domain.Log.Contracts;

namespace RS.Logstream.API.Endpoints;

public static class LogEndpoints
{
	public static WebApplication MapLogEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/logs").RequireAuthorization();

		group.MapGet("/", (
			[FromServices] ILogRepository logRepository,
			[FromQuery(Name = "pn")] int? pageNumber,
			[FromQuery(Name = "ps")] int? pageSize,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			var logList = logRepository.GetAll(pageNumber, pageSize, tenantId);
			return Results.Ok(logList);
		});

		group.MapGet("/{id}", (
			[FromServices] ILogRepository logRepository,
			long id,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			var log = logRepository.GetById(id, tenantId);
			return Results.Ok(log);
		});

		group.MapGet("/search", (
			[FromServices] ILogRepository logRepository,
			[FromQuery(Name = "ds")] DateTime? dateStart,
			[FromQuery(Name = "de")] DateTime? dateEnd,
			[FromQuery(Name = "ll")] int? logLevel,
			[FromQuery(Name = "msg")] string? message,
			[FromQuery(Name = "pn")] int? pageNumber,
			[FromQuery(Name = "ps")] int? pageSize,
			[FromQuery(Name = "q")] string? fullTextQuery,
			[FromQuery(Name = "cid")] string? correlationId,
			[FromQuery(Name = "tid")] string? traceId,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			LogLevel? ll = logLevel is null ? null : (LogLevel)logLevel;
			var logList = logRepository.Search(dateStart, dateEnd, ll, message, pageNumber, pageSize, tenantId, correlationId, traceId, fullTextQuery);
			return Results.Ok(logList);
		});

		group.MapPost("/", (
			[FromServices] ILogIngestionQueue ingestionQueue,
			[FromBody] LogsViewModel pModel,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId,
			[FromHeader(Name = "X-Correlation-Id")] string? correlationId,
			[FromHeader(Name = "X-Trace-Id")] string? traceId) =>
		{
			var log = new Log(pModel.LogLevel, pModel.Message, pModel.StackTrace, tenantId, correlationId, traceId);
			ingestionQueue.Enqueue((services, _) =>
			{
				services.GetRequiredService<ILogRepository>().Create(log);
				return Task.CompletedTask;
			});
			return Results.Accepted();
		});

		group.MapPost("/batch", (
			[FromServices] ILogIngestionQueue ingestionQueue,
			[FromBody] List<LogsViewModel> pModels,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId,
			[FromHeader(Name = "X-Correlation-Id")] string? correlationId,
			[FromHeader(Name = "X-Trace-Id")] string? traceId) =>
		{
			foreach (var pModel in pModels)
			{
				var log = new Log(pModel.LogLevel, pModel.Message, pModel.StackTrace, tenantId, correlationId, traceId);
				ingestionQueue.Enqueue((services, _) =>
				{
					services.GetRequiredService<ILogRepository>().Create(log);
					return Task.CompletedTask;
				});
			}
			return Results.Accepted();
		});

		return app;
	}
}
