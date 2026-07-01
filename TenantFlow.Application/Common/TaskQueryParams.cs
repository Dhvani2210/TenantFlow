using TaskStatus = TenantFlow.Domain.Enums.TaskStatus;
namespace TenantFlow.Application.Common;

public class TaskQueryParams : PaginationParams
{
    public string? Search { get; set; }
    public TaskStatus? Status { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}