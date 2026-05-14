using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantFlow.Application.Interfaces;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Infrastructure.Repositories;
using TenantFlow.Infrastructure.Services;
using TenantFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace TenantFlow.Infrastructure;

public static class DependencyInjection
{
    // 'this IServiceCollection services' is what makes this an extension method.
    // The 'this' keyword means you can call it as services.AddInfrastructure(...)
    // instead of DependencyInjection.AddInfrastructure(services, ...)
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TenantFlowDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging());

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register the concrete type as scoped first
        services.AddScoped<TenantContext>();
        services.AddScoped<IProjectService, ProjectService>();

        // Both interfaces resolve to the same instance within a request
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantContextSetter>(sp => sp.GetRequiredService<TenantContext>());

        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}