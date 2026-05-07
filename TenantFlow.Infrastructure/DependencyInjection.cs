using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantFlow.Application.Interfaces;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Infrastructure.Repositories;
using TenantFlow.Infrastructure.Services;
using TenantFlow.Application.Common.Interfaces;

namespace TenantFlow.Infrastructure;

public static class DependencyInjection
{
    // 'this IServiceCollection services' is what makes this an extension method.
    // The 'this' keyword means you can call it as services.AddInfrastructure(...)
    // instead of DependencyInjection.AddInfrastructure(services, ...)
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TenantFlowDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProjectRepository, ProjectRepository>();

        // Register the concrete type as scoped first
        services.AddScoped<TenantContext>();

        // Both interfaces resolve to the same instance within a request
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantContextSetter>(sp => sp.GetRequiredService<TenantContext>());

        return services;
    }
}