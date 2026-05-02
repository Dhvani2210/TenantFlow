namespace TenantFlow.Domain.Entities;

public class Task
{
    public Guid TaskId { get; set; }
    public Guid TenantId { get; set; }      // direct tenant scope — used for query filtering
    public Guid ProjectId { get; set; }
    public Guid? AssignedToUserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public User? AssignedTo { get; set; }
}