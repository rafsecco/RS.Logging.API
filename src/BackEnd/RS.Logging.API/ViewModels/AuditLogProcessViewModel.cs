using Microsoft.Extensions.Logging;
using RS.Logging.Domain.LogProcess;

namespace RS.Logging.API.ViewModels;

public record AuditLogProcessDetailViewModel(
    ulong Id,
    LogLevel LogLevel,
    string Message,
    string? StackTrace,
    DateTime CreatedAt
);

public record AuditLogProcessViewModel(
    ulong Id,
    uint ProcessId,
    string? Name,
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
            process.CreatedAt,
            process.GetStatus(),
            process.LorProcessDetailList?.Select(d => new AuditLogProcessDetailViewModel(
                d.Id,
                d.LogLevel,
                d.Message,
                d.StackTrace,
                d.CreatedAt
            )) ?? []
        );
}
