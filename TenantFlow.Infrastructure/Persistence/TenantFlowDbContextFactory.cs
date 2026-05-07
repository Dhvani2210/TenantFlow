using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TenantFlow.Application.Common.Interfaces;

namespace TenantFlow.Infrastructure.Persistence;

public class TenantFlowDbContextFactory : IDesignTimeDbContextFactory<TenantFlowDbContext>
{
    public TenantFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantFlowDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=LAPTOP-ITSG6C0Q\\Dhvni,1433;Database=TenantFlowDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new TenantFlowDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }

    // Private stub — invisible outside this file.
    // Returns Guid.Empty because EF tooling only needs the schema, never real tenant data.
    private class DesignTimeTenantContext : ITenantContext
    {
        public Guid TenantId => Guid.Empty;
    }
}