namespace RS.Core.Pagination;

public static class PaginationHelper
{
	public static int GetSkip(int pageNumber, int pageSize) => (pageNumber - 1) * pageSize;
}
