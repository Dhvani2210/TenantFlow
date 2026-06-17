namespace TenantFlow.Domain.Entities;

public class Tenant
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<User> Users { get; set; } = new List<User>();
}