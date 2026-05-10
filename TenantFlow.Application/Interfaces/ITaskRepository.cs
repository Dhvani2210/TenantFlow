using TenantFlow.Domain.Entities;

namespace TenantFlow.Application.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<Domain.Entities.Task>> GetAllAsync(Guid projectId);
    Task<Domain.Entities.Task?> GetByIdAsync(Guid id, Guid projectId);
    Task<Domain.Entities.Task> CreateAsync(Domain.Entities.Task task);
    Task<Domain.Entities.Task?> UpdateAsync(Domain.Entities.Task task);
    Task<bool> DeleteAsync(Guid id, Guid projectId);
}