using RS.Core.Entities;

namespace RS.Logstream.Domain.ApiCall;

public class ApiCallLog : BaseEntity
{
	protected ApiCallLog() { }

	public ApiCallLog(
		string pUrl,
		string pHttpMethod,
		bool pIsSuccess,
		string? pRequestBody = null,
		string? pRequestHeaders = null,
		int? pResponseStatusCode = null,
		string? pResponseBody = null,
		long? pDurationMs = null,
		string? pErrorMessage = null,
		string? pTenantId = null,
		string? pCorrelationId = null,
		string? pTraceId = null)
	{
		Url = pUrl;
		HttpMethod = pHttpMethod;
		IsSuccess = pIsSuccess;
		RequestBody = pRequestBody;
		RequestHeaders = pRequestHeaders;
		ResponseStatusCode = pResponseStatusCode;
		ResponseBody = pResponseBody;
		DurationMs = pDurationMs;
		ErrorMessage = pErrorMessage;
		TenantId = pTenantId;
		CorrelationId = pCorrelationId;
		TraceId = pTraceId;
	}

	public string Url { get; private set; } = null!;
	public string HttpMethod { get; private set; } = null!;
	public bool IsSuccess { get; private set; }
	public string? RequestBody { get; private set; }
	public string? RequestHeaders { get; private set; }
	public int? ResponseStatusCode { get; private set; }
	public string? ResponseBody { get; private set; }
	public long? DurationMs { get; private set; }
	public string? ErrorMessage { get; private set; }
	public string? TenantId { get; private set; }
	public string? CorrelationId { get; private set; }
	public string? TraceId { get; private set; }
}
