namespace TenantFlow.Domain.Entities;

public class User
{
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // bcrypt hash — never store plaintext
    public string Role { get; set; } = string.Empty;         // "Admin", "Manager", "Developer"
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }

    public Tenant Tenant { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}