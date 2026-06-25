using Microsoft.AspNetCore.Mvc;
using RS.Logstream.API.ViewModels;
using RS.Logstream.Domain.LogProcess;
using RS.Logstream.Domain.LogProcess.Contracts;

namespace RS.Logstream.API.Endpoints;

public static class AuditEndpoints
{
	public static WebApplication MapAuditEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/log-process/audit").RequireAuthorization();

		group.MapGet("/", (
			[FromServices] ILogProcessRepository logProcessRepository,
			[FromQuery(Name = "ds")] DateTime? dateStart,
			[FromQuery(Name = "de")] DateTime? dateEnd,
			[FromQuery(Name = "st")] int? status,
			[FromQuery(Name = "pn")] int? pageNumber,
			[FromQuery(Name = "ps")] int? pageSize,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			ProcessStatus? processStatus = status.HasValue ? (ProcessStatus)status.Value : null;
			var result = logProcessRepository
				.GetAudit(dateStart, dateEnd, processStatus, pageNumber, pageSize, tenantId)
				.Select(AuditLogProcessViewModel.FromDomain);
			return Results.Ok(result);
		});

		group.MapGet("/{id}", (
			[FromServices] ILogProcessRepository logProcessRepository,
			ulong id,
			[FromHeader(Name = "X-Tenant-Id")] string? tenantId) =>
		{
			var process = logProcessRepository.GetById(id, tenantId);
			if (process is null)
				return Results.NotFound();
			return Results.Ok(AuditLogProcessViewModel.FromDomain(process));
		});

		return app;
	}
}
