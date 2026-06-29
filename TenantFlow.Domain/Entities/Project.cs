namespace TenantFlow.Domain.Entities;

public class Project
{
    public Guid ProjectId { get; init; }
    public Guid TenantId { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }  
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property UP to the owning Tenant
    public Tenant Tenant { get; set; } = null!;

    // Navigation property DOWN to owned Tasks
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}