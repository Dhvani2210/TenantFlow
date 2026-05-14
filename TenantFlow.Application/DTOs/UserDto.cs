namespace TenantFlow.Application.DTOs;

// Response DTO for a user record scoped to the current tenant.
// PasswordHash is deliberately excluded — it must never leave the server.
// TenantId is excluded — it's an internal isolation column.
public class UserDto
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

// Inbound DTO for creating a new user under the current tenant.
// TenantId is NOT here — the service reads it from ITenantContext.
// Role is included because the admin creating the user assigns their role.
// PasswordHash is NOT here — the service will hash the raw password before storing.
public class CreateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

// Inbound DTO for updating an existing user.
// Password and Role are excluded — password change and role change are
// sensitive operations that belong to dedicated endpoints, not a general update.
// TenantId and UserId are excluded — those are never caller-supplied on updates.
public class UpdateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}