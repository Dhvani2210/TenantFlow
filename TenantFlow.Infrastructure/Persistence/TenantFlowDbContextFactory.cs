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
        var optionsBuilder = new DbContextOptionsBuilder<TenantFlowDbContext>();

        // Hardcoded connection string for EF Core tooling only.
        // This class is never called at runtime — DI handles that.
        optionsBuilder.UseSqlServer(
            "Server=LAPTOP-ITSG6C0Q\\Dhvni,1433;Database=TenantFlowDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new TenantFlowDbContext(optionsBuilder.Options);
    }
}