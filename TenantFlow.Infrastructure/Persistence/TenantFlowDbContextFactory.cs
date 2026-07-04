using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TenantFlow.Application.Common.Interfaces;

namespace TenantFlow.Infrastructure.Persistence;

public class TenantFlowDbContextFactory : IDesignTimeDbContextFactory<TenantFlowDbContext>
{
    public TenantFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantFlowDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=TenantFlowDb;Username=postgres;Password=admin");

        return new TenantFlowDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }

    // Private stub — invisible outside this file.
    // Returns Guid.Empty because EF tooling only needs the schema, never real tenant data.
    private class DesignTimeTenantContext : ITenantContext
    {
        public Guid TenantId => Guid.Empty;
    }
}