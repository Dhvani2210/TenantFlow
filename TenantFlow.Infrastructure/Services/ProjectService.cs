using TenantFlow.Application.Common;
using TenantFlow.Application.Common.Interfaces;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly ITenantContext _tenantContext;

    public ProjectService(IProjectRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<IEnumerable<ProjectDto>>> GetAllAsync()
    {
        try
        {
            var projects = await _repository.GetAllAsync();
            return Result<IEnumerable<ProjectDto>>.Success(projects.Select(MapToDto));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProjectDto>>.Failure(
                $"Failed to retrieve projects: {ex.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var project = await _repository.GetByIdAsync(id);

            if (project is null)
                return Result<ProjectDto>.Failure(
                    "Project not found.",
                    ErrorType.NotFound);

            return Result<ProjectDto>.Success(MapToDto(project));
        }
        catch (Exception ex)
        {
            return Result<ProjectDto>.Failure(
                $"Failed to retrieve project: {ex.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto)
    {
        try
        {
            var project = new Project
            {
                ProjectId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                TenantId = _tenantContext.TenantId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var created = await _repository.CreateAsync(project);
            return Result<ProjectDto>.Success(MapToDto(created));
        }
        catch (Exception ex)
        {
            return Result<ProjectDto>.Failure(
                $"Failed to create project: {ex.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing is null)
                return Result<ProjectDto>.Failure(
                    "Project not found.",
                    ErrorType.NotFound);

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return Result<ProjectDto>.Success(MapToDto(updated!));
        }
        catch (Exception ex)
        {
            return Result<ProjectDto>.Failure(
                $"Failed to update project: {ex.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return Result<bool>.Failure(
                    "Project not found.",
                    ErrorType.NotFound);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                $"Failed to delete project: {ex.Message}",
                ErrorType.ServerError);
        }
    }

    // Mapping lives here — ProjectMappings.cs is deleted.
    // The service is the only consumer of this logic, so it owns it.
    private static ProjectDto MapToDto(Project p) => new()
    {
        Id = p.ProjectId,
        TenantId = p.TenantId,
        Name = p.Name,
        Description = p.Description,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}