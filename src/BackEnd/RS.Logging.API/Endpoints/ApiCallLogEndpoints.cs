using Microsoft.AspNetCore.Mvc;
using RS.Logging.API.Ingestion;
using RS.Logging.API.ViewModels;
using RS.Logging.Domain.ApiCall;
using RS.Logging.Domain.ApiCall.Contracts;

namespace RS.Logging.API.Endpoints;

public static class ApiCallLogEndpoints
{
    public static WebApplication MapApiCallLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api-call-log").RequireAuthorization();

        group.MapGet("/", (
            [FromServices] IApiCallLogRepository repository,
            [FromQuery(Name = "pn")] int? pageNumber,
            [FromQuery(Name = "ps")] int? pageSize,
            [FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
        {
            var result = repository.GetAll(pageNumber, pageSize, tenantId);
            return Results.Ok(result);
        });

        group.MapGet("/{id}", (
            [FromServices] IApiCallLogRepository repository,
            ulong id,
            [FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
        {
            var result = repository.GetById(id, tenantId);
            return Results.Ok(result);
        });

        group.MapGet("/search", (
            [FromServices] IApiCallLogRepository repository,
            [FromQuery(Name = "ds")] DateTime? dateStart,
            [FromQuery(Name = "de")] DateTime? dateEnd,
            [FromQuery(Name = "ok")] bool? isSuccess,
            [FromQuery(Name = "sc")] int? statusCode,
            [FromQuery(Name = "url")] string? url,
            [FromQuery(Name = "pn")] int? pageNumber,
            [FromQuery(Name = "ps")] int? pageSize,
            [FromQuery(Name = "cid")] string? correlationId,
            [FromQuery(Name = "tid")] string? traceId,
            [FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
        {
            var result = repository.Search(dateStart, dateEnd, isSuccess, statusCode, url, pageNumber, pageSize, tenantId, correlationId, traceId);
            return Results.Ok(result);
        });

        group.MapPost("/", (
            [FromServices] ILogIngestionQueue ingestionQueue,
            [FromBody] ApiCallLogViewModel pModel,
            [FromHeader(Name = "X-Tenant-Id")] string? tenantId,
            [FromHeader(Name = "X-Correlation-Id")] string? correlationId,
            [FromHeader(Name = "X-Trace-Id")] string? traceId) =>
        {
            var entry = new ApiCallLog(
                pModel.Url,
                pModel.HttpMethod,
                pModel.IsSuccess,
                pModel.RequestBody,
                pModel.RequestHeaders,
                pModel.ResponseStatusCode,
                pModel.ResponseBody,
                pModel.DurationMs,
                pModel.ErrorMessage,
                tenantId,
                correlationId,
                traceId);

            ingestionQueue.Enqueue((services, _) =>
            {
                services.GetRequiredService<IApiCallLogRepository>().Create(entry);
                return Task.CompletedTask;
            });

            return Results.Accepted();
        });

        return app;
    }
}
