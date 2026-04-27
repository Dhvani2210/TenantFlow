using TenantFlow.Api.DTOs;
namespace TenantFlow.Api.Repositories.Interfaces;

public interface IProjectRepository
{
    // Every method takes tenantId — tenant isolation is enforced
    // at this layer, not left to individual callers to remember.
    Task<IEnumerable<ProjectDto>> GetAllAsync(Guid tenantId);
    Task<ProjectDto?> GetByIdAsync(Guid id, Guid tenantId);
    Task<ProjectDto> CreateAsync(Guid tenantId, CreateProjectDto dto);
    Task<ProjectDto?> UpdateAsync(Guid id, Guid tenantId, UpdateProjectDto dto);
    Task<bool> DeleteAsync(Guid id, Guid tenantId);
}