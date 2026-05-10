namespace TenantFlow.Application.DTOs;

// This is the shape of a Project as the outside world sees it.
// No navigation properties, no EF Core concerns, no IsDeleted flag.
// Just the data a client needs to display or work with a project.
public class ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}

// Separate DTO for creation — the client provides Name and Description.
// The API assigns Id, TenantId, and CreatedAt — the client never should.
public class CreateProjectDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class UpdateProjectDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}