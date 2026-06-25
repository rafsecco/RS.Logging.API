using Microsoft.Extensions.Logging;
using RS.Logstream.Domain.LogProcess;

namespace RS.Logstream.API.ViewModels;

public record AuditLogProcessDetailViewModel(
	ulong Id,
	LogLevel LogLevel,
	string Message,
	string? StackTrace,
	string? CorrelationId,
	string? TraceId,
	DateTime CreatedAt
);

public record AuditLogProcessViewModel(
	ulong Id,
	uint ProcessId,
	string? Name,
	string? TenantId,
	string? CorrelationId,
	string? TraceId,
	DateTime CreatedAt,
	ProcessStatus Status,
	IEnumerable<AuditLogProcessDetailViewModel> Details
)
{
	public static AuditLogProcessViewModel FromDomain(LogProcess process) =>
		new(
			process.Id,
			process.ProcessId,
			process.Name,
			process.TenantId,
			process.CorrelationId,
			process.TraceId,
			process.CreatedAt,
			process.GetStatus(),
			process.LorProcessDetailList?.Select(d => new AuditLogProcessDetailViewModel(
				d.Id,
				d.LogLevel,
				d.Message,
				d.StackTrace,
				d.CorrelationId,
				d.TraceId,
				d.CreatedAt
			)) ?? []
		);
}
