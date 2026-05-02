namespace TenantFlow.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // bcrypt hash — never store plaintext
    public string Role { get; set; } = string.Empty;         // "Admin", "Manager", "Developer"
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}