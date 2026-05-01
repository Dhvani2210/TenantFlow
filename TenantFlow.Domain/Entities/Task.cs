namespace TenantFlow.Domain.Entities;

public class Task
{
    public Guid TaskId { get; set; }
    public Guid ProjectId { get; set; }

    // Nullable FK — a task may or may not be assigned to a user
    // The ? makes the Guid itself nullable, mirroring NULL allowed in SQL
    public Guid? AssignedToUserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation UP to owning Project
    public Project Project { get; set; } = null!;

    // Navigation UP to assigned User — nullable because assignment is optional
    public User? AssignedTo { get; set; }
}