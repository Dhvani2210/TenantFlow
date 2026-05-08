using TenantFlow.Application.DTOs;
using TenantFlow.Application.Common;

namespace TenantFlow.Application.Interfaces;

public interface IProjectService
{
    Task<Result<IEnumerable<ProjectDto>>> GetAllAsync();
    Task<Result<ProjectDto>> GetByIdAsync(Guid id);
    Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto);
    Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}