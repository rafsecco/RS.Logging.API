using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RS.Logstream.Tests.Integration.Helpers;

public static class TestJson
{
	public static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
}

public record LogDto(
	long Id,
	LogLevel LogLevel,
	string Message,
	string? StackTrace,
	string? TenantId,
	string? CorrelationId,
	string? TraceId,
	DateTime CreatedAt);

public record LogProcessDetailDto(
	long Id,
	long LogProcessId,
	LogLevel LogLevel,
	string Message,
	string? StackTrace,
	string? CorrelationId,
	string? TraceId,
	DateTime CreatedAt);

public record LogProcessDto(
	long Id,
	uint ProcessId,
	string? Name,
	string? TenantId,
	string? CorrelationId,
	string? TraceId,
	DateTime CreatedAt,
	List<LogProcessDetailDto>? LorProcessDetailList);

public record ApiCallLogDto(
	long Id,
	string Url,
	string HttpMethod,
	bool IsSuccess,
	string? RequestBody,
	string? RequestHeaders,
	int? ResponseStatusCode,
	string? ResponseBody,
	long? DurationMs,
	string? ErrorMessage,
	string? TenantId,
	string? CorrelationId,
	string? TraceId,
	DateTime CreatedAt);
