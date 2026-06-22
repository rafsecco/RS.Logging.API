namespace RS.Logging.API.ViewModels;

public record ApiCallLogViewModel(
    string Url,
    string HttpMethod,
    bool IsSuccess,
    string? RequestBody,
    string? RequestHeaders,
    int? ResponseStatusCode,
    string? ResponseBody,
    long? DurationMs,
    string? ErrorMessage
);
