using TenantFlow.Application.Common;
using TenantFlow.Application.DTOs;

namespace TenantFlow.Application.Interfaces;

public interface ITaskService
{
    Task<Result<PagedResult<TaskDto>>> GetAllAsync(Guid projectId, TaskQueryParams queryParams);
    Task<Result<TaskDto>> GetByIdAsync(Guid id, Guid projectId);
    Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto);
    Task<Result<TaskDto>> UpdateAsync(Guid id, Guid projectId, UpdateTaskDto dto);
    Task<Result<bool>> DeleteAsync(Guid id, Guid projectId);
}