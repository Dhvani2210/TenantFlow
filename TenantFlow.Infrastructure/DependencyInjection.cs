using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantFlow.Application.Interfaces;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Infrastructure.Repositories;

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

        return services;
    }
}