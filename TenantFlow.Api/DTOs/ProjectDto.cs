namespace TenantFlow.Api.DTOs;

// This is the shape of a Project as the outside world sees it.
// No navigation properties, no EF Core concerns, no IsDeleted flag.
// Just the data a client needs to display or work with a project.
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid TenantId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

// Separate DTO for creation — the client provides Name and Description.
// The API assigns Id, TenantId, and CreatedAt — the client never should.
public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}