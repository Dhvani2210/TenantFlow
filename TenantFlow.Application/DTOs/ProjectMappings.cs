using TenantFlow.Application.DTOs;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Application.DTOs;

public static class ProjectMappings
{
    // Centralising mapping here keeps it out of the controller.
    // When ProjectService is introduced, this logic moves there
    // and this class is deleted.
    public static ProjectDto ToDto(Project p) => new()
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