namespace TenantFlow.Domain.Entities;

public class Tenant
{
    // Mirrors TenantId (PK, uniqueidentifier, NEWSEQUENTIALID())
    public Guid TenantId { get; set; }

    // Mirrors Name (nvarchar(100), NOT NULL)
    public string Name { get; set; } = string.Empty;

    // Mirrors Email (nvarchar(255), NOT NULL)
    public string Email { get; set; } = string.Empty;

    // Mirrors IsActive (bit, DEFAULT 1)
    public bool IsActive { get; set; }

    // Mirrors CreatedAt (datetime2, DEFAULT GETUTCDATE())
    public DateTime CreatedAt { get; set; }

    // Navigation properties — the "one" side of one-to-many relationships
    // EF Core uses these to understand that a Tenant owns many Projects and Users
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<User> Users { get; set; } = new List<User>();
}