namespace RS.Core.Entities;

public class PagedModel<T> where T : class
{
	public IEnumerable<T>? List { get; private set; }

	public ushort PageIndex { get; set; } = 1;

	public ushort PageSize { get; set; } = 10;

	public int TotalResults { get; private set; }

	public ushort TotalPages => Convert.ToUInt16(Math.Ceiling((double)TotalResults / PageSize));

	protected PagedModel() { }

	public PagedModel(int pTotalResults, ushort pPageIndex = 1, ushort pPageSize = 10, IEnumerable<T>? pList = null)
	{
		List = pList;
		TotalResults = pTotalResults;
		PageIndex = pPageIndex;
		PageSize = pPageSize;
	}
}
