namespace TenantFlow.Application.Common;
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    // Derived, not stored — can never drift out of sync with TotalCount/PageSize
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}