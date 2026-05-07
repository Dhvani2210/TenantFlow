namespace TenantFlow.Application.Common.Interfaces;

public interface ITenantContext
{
    // The resolved TenantId for the current HTTP request.
    // Guid.Empty means no tenant has been resolved yet — treat as unauthenticated.
    Guid TenantId { get; }
}