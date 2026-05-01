using TenantFlow.Api.Entities;

namespace TenantFlow.Api.Repositories.Interfaces;
public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync(Guid tenantId);
    Task<Project?> GetByIdAsync(Guid id, Guid tenantId);
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(Project project);
    Task<bool> DeleteAsync(Guid id, Guid tenantId);
}