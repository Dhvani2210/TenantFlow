using FluentValidation;
using TenantFlow.Application.Common;
using TenantFlow.Application.Common.Interfaces;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;
using TaskStatus = TenantFlow.Domain.Enums.TaskStatus;


namespace TenantFlow.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly IProjectRepository _projectRepository;  
        private readonly ITaskRepository _taskRepository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateTaskDto> _validator;
        private readonly IHubNotificationService _hubNotificationService;


        public TaskService(IProjectRepository projectRepository, ITaskRepository taskRepository,
                ITenantContext tenantContext, IValidator<CreateTaskDto> validator, IHubNotificationService hubNotificationService)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _tenantContext = tenantContext;
            _validator = validator;
            _hubNotificationService = hubNotificationService;
        }

        private static TaskDto MapToDto(Domain.Entities.Task t) => new()
        {
            TaskId = t.TaskId,
            ProjectId = t.ProjectId,
            Name = t.Name,
            Description = t.Description,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            AssignedToUserId = t.AssignedToUserId,
            AssignedToUserName = t.AssignedTo?.FullName,
            UpdatedAt = t.UpdatedAt,
            Status = t.Status.ToString()
        };

        public async Task<Result<PagedResult<TaskDto>>> GetAllAsync(Guid projectId, TaskQueryParams queryParams)
        {
            try
            {
                var pagedTasks = await _taskRepository.GetAllAsync(projectId, queryParams);
                var pagedDto = new PagedResult<TaskDto>
                {
                    Data = pagedTasks.Data.Select(MapToDto),
                    TotalCount = pagedTasks.TotalCount,
                    PageNumber = pagedTasks.PageNumber,
                    PageSize = pagedTasks.PageSize
                };
                return Result<PagedResult<TaskDto>>.Success(pagedDto);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<TaskDto>>.Failure(
                    $"Failed to retrieve tasks: {ex.Message}",
                    ErrorType.ServerError);
            }
        }

        public async Task<Result<TaskDto>> GetByIdAsync(Guid id, Guid projectId)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);

                if (project is null)
                    return Result<TaskDto>.Failure(
                        "Project not found.",
                        ErrorType.NotFound);
                var task = await _taskRepository.GetByIdAsync(id, projectId);
                if (task is null)
                    return Result<TaskDto>.Failure(
                       "Task not found.",
                       ErrorType.NotFound);

                return Result<TaskDto>.Success(MapToDto(task));
            }
            catch (Exception ex)
            {
                return Result<TaskDto>.Failure(
                    $"Failed to retrieve task: {ex.Message}",
                    ErrorType.ServerError);
            }
        }

        public async Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);

                if (project is null)
                    return Result<TaskDto>.Failure(
                       "Project not found.",
                       ErrorType.NotFound);

                var validationResult = await _validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result<TaskDto>.Failure(errors, ErrorType.Validation);
                }

                var task = new Domain.Entities.Task
                    {
                        TaskId = Guid.NewGuid(),
                        ProjectId = projectId,
                        Name = dto.Name,
                        Description = dto.Description,
                        TenantId = _tenantContext.TenantId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        DueDate = dto.DueDate,
                        AssignedToUserId = dto.AssignedToUserId,
                        Status = TaskStatus.Todo

                    };

                    var created = await _taskRepository.CreateAsync(task);
                    var taskDto = MapToDto(created);
                    await _hubNotificationService.NotifyTaskCreated(_tenantContext.TenantId.ToString(), taskDto);
                    return Result<TaskDto>.Success(taskDto);
            }
            catch (Exception ex)
            {
                return Result<TaskDto>.Failure(
               $"Failed to create Task: {ex.Message}",
               ErrorType.ServerError);
            }
        }

        public async Task<Result<TaskDto>> UpdateAsync(Guid id, Guid projectId, UpdateTaskDto dto)
        {
            try
            {
                var existing = await _projectRepository.GetByIdAsync(projectId);
                if(existing is null)
                    return Result<TaskDto>.Failure(
                      "Project not found.",
                      ErrorType.NotFound);

                var existingTask = await _taskRepository.GetByIdAsync(id, projectId);

                if(existingTask is null)
                    return Result<TaskDto>.Failure(
                      "Task not found.",
                      ErrorType.NotFound);

                existingTask.Name = dto.Name;
                existingTask.Description = dto.Description;
                existingTask.DueDate = dto.DueDate;
                existingTask.AssignedToUserId = dto.AssignedToUserId;
                existingTask.UpdatedAt = DateTime.UtcNow;
                existingTask.Status = Enum.Parse<TaskStatus>(dto.Status);

                var updatedTask = await _taskRepository.UpdateAsync(existingTask);
                var taskDto = MapToDto(updatedTask!);
                await _hubNotificationService.NotifyTaskUpdated(_tenantContext.TenantId.ToString(), taskDto);
                return Result<TaskDto>.Success(taskDto);

            }
            catch (Exception ex)
            {
                return Result<TaskDto>.Failure(
                    $"Failed to update Task: {ex.Message}",
                    ErrorType.ServerError);
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id, Guid projectId)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project is null)
                    return Result<bool>.Failure(
                        "Project not found.",
                        ErrorType.NotFound);

                var deleted = await _taskRepository.DeleteAsync(id, projectId);
                if (!deleted)
                    return Result<bool>.Failure(
                        "Task not found.",
                        ErrorType.NotFound);

                await _hubNotificationService.NotifyTaskDeleted(_tenantContext.TenantId.ToString(), id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(
                    $"Failed to delete task: {ex.Message}",
                    ErrorType.ServerError);
            }
        }
    }
}
