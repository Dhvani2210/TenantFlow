namespace TenantFlow.Api.Entities;

public class Project
{
    public Guid ProjectId { get; set; }

    // Foreign key property — holds the raw Guid value stored in the column
    public Guid TenantId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }  // nullable — mirrors NULL allowed in SQL
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property UP to the owning Tenant
    public Tenant Tenant { get; set; } = null!;

    // Navigation property DOWN to owned Tasks
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}