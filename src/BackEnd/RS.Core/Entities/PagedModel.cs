namespace RS.Core.Entities;

public class PagedModel<T> where T : class
{
	public PagedModel(int pTotalResults, ushort pPageNumber = 1, ushort pPageSize = 10, IEnumerable<T>? pList = null)
	{
		TotalResults = pTotalResults;
		PageNumber = pPageNumber;
		PageSize = pPageSize;
		List = pList;
	}

	protected PagedModel() { }

	public ushort PageSize { get; set; } = 10;
	public ushort PageNumber { get; set; } = 1;
	public int TotalResults { get; private set; }
	public ushort TotalPages => Convert.ToUInt16(Math.Ceiling((double)TotalResults / PageSize));
	public IEnumerable<T>? List { get; private set; }
}
