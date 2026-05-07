using TenantFlow.Application.Common.Interfaces;

namespace TenantFlow.Infrastructure.Services;

// This internal interface is the "write side" — only the middleware uses it.
// Nothing in Application ever sees this; it only knows ITenantContext (read-only).
public interface ITenantContextSetter
{
    void SetTenantId(Guid tenantId);
}

public class TenantContext : ITenantContext, ITenantContextSetter
{
    // Backing field — private set means only this class can assign directly.
    private Guid _tenantId;

    // ITenantContext — read-only to the outside world
    public Guid TenantId => _tenantId;

    // ITenantContextSetter — only the middleware injects and calls this
    public void SetTenantId(Guid tenantId) => _tenantId = tenantId;
}