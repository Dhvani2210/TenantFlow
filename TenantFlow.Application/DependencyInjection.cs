using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Scans the Application assembly and registers all AbstractValidator<T>
        // implementations automatically. No need to register each validator manually.
        services.AddValidatorsFromAssemblyContaining<IProjectService>();

        return services;
    }
}