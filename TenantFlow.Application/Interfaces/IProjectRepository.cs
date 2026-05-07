using TenantFlow.Domain.Entities;

namespace TenantFlow.Application.Interfaces;
public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id, Guid tenantId);
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(Project project);
    Task<bool> DeleteAsync(Guid id, Guid tenantId);
}