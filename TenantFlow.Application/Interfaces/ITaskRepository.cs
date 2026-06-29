using TenantFlow.Application.Common;

namespace TenantFlow.Application.Interfaces;

public interface ITaskRepository
{
    Task<PagedResult<Domain.Entities.Task>> GetAllAsync(Guid projectId, PaginationParams paginationParams);
    Task<Domain.Entities.Task?> GetByIdAsync(Guid id, Guid projectId);
    Task<Domain.Entities.Task> CreateAsync(Domain.Entities.Task task);
    Task<Domain.Entities.Task?> UpdateAsync(Domain.Entities.Task task);
    Task<bool> DeleteAsync(Guid id, Guid projectId);
}