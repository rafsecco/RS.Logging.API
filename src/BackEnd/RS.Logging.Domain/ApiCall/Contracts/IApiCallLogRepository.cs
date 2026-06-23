namespace RS.Logging.Domain.ApiCall.Contracts;

public interface IApiCallLogRepository
{
    bool Create(ApiCallLog apiCallLog);
    ApiCallLog? GetById(ulong id, string? tenantId = null);
    IEnumerable<ApiCallLog> GetAll(int? page, int? pageSize, string? tenantId = null);
    IEnumerable<ApiCallLog> Search(
        DateTime? dateStart = null,
        DateTime? dateEnd = null,
        bool? isSuccess = null,
        int? responseStatusCode = null,
        string? url = null,
        int? pageNumber = null,
        int? pageSize = null,
        string? tenantId = null,
        string? correlationId = null,
        string? traceId = null);
}
