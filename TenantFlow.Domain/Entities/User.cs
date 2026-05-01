namespace TenantFlow.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property UP to the owning Tenant
    public Tenant Tenant { get; set; } = null!;

    // Navigation property DOWN to assigned Tasks
    // A user can be assigned to many tasks
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}