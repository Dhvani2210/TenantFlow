namespace TenantFlow.Domain.Entities;

public class Task
{
    public Guid TaskId { get; init; }
    public Guid TenantId { get; init; }      // direct tenant scope — used for query filtering
    public Guid ProjectId { get; init; }
    public Guid? AssignedToUserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }

    public Project Project { get; set; } = null!;
    public User? AssignedTo { get; set; }
}