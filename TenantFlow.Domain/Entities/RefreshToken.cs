namespace TenantFlow.Domain.Entities;

public class RefreshToken
{
    public Guid RefreshTokenId { get; init; }
    public Guid UserId { get; init; }
    public string TokenHash { get; set; } = string.Empty; 
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}