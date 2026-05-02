using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TenantFlow.Infrastructure.Persistence;

// This class is only used by EF Core tooling (dotnet ef migrations add, etc.)
// It is never called at runtime — the DI container handles DbContext creation there
public class TenantFlowDbContextFactory : IDesignTimeDbContextFactory<TenantFlowDbContext>
{
    public TenantFlowDbContext CreateDbContext(string[] args)
    {
        // Walk up from Infrastructure/bin to find the API project's appsettings.json
        // This is necessary because EF tooling runs from the Infrastructure project directory
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../TenantFlow.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TenantFlowDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new TenantFlowDbContext(optionsBuilder.Options);
    }
}