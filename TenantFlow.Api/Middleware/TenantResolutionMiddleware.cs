using TenantFlow.Infrastructure.Services;

namespace TenantFlow.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    // RequestDelegate is the next middleware in the pipeline.
    // We inject it through the constructor because middleware is instantiated
    // once at startup — Singleton. it is NOT scoped per request. This is why we cannot
    // inject ITenantContextSetter here in the constructor; it's scoped and
    // would cause a captive dependency problem. We resolve it per-request below.
    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContextSetter tenantContextSetter)
    {


        // Skip tenant resolution for Scalar UI and OpenAPI spec routes
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/scalar") || path.StartsWith("/openapi"))
        {
            await _next(context);
            return;
        }


        // InvokeAsync receives scoped services as parameters — ASP.NET Core
        // resolves them fresh from the request's DI scope each time.
        // This is the correct way to use scoped services inside middleware.

        // HttpContext.User is already populated by UseAuthentication().
        // We just need to find the TenantId claim by its name.
        var tenantIdClaim = context.User.FindFirst("TenantId");

        // if there's no tenantId claim, just call _next and continue.
        // Let the endpoint's own [AllowAnonymous] or [Authorize] attribute handle authorization.
        // The middleware's only job is tenant resolution — not access control.
        if (tenantIdClaim is null || !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            await _next(context);
            return;
        }

        // Set the resolved TenantId on the scoped context so every downstream
        // class that injects ITenantContext gets it automatically.
        tenantContextSetter.SetTenantId(tenantId);

        // Pass control to the next middleware in the pipeline.
        await _next(context);
    }
}