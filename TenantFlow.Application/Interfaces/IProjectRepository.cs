using TenantFlow.Application.Common;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Application.Interfaces;
public interface IProjectRepository
{
    Task<PagedResult<Project>> GetAllAsync(PaginationParams paginationParams);
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(Project project);
    Task<bool> DeleteAsync(Guid id);
}