using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TenantFlow.Application.Common.Interfaces;
using TenantFlow.Application.Interfaces;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Infrastructure.Repositories;
using TenantFlow.Infrastructure.Services;

namespace TenantFlow.Infrastructure;

public static class DependencyInjection
{
    // 'this IServiceCollection services' is what makes this an extension method.
    // The 'this' keyword means you can call it as services.AddInfrastructure(...)
    // instead of DependencyInjection.AddInfrastructure(services, ...)
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContext<TenantFlowDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            if (environment.IsDevelopment())
            {
                options.LogTo(Console.WriteLine, LogLevel.Information)
                       .EnableSensitiveDataLogging();
            }
        });

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

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}