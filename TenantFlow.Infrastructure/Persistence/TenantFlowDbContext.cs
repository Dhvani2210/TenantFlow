using Microsoft.EntityFrameworkCore;
using TenantFlow.Application.Common.Interfaces;
using TenantFlow.Persistence.Configurations;
using TenantFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TenantFlow.Infrastructure.Persistence;

public class TenantFlowDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public TenantFlowDbContext(
        DbContextOptions<TenantFlowDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Domain.Entities.Task> Tasks => Set<Domain.Entities.Task>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantFlowDbContext).Assembly);

        // Global query filters — automatically appended as WHERE clauses on every
        // query for these entities. Tenant isolation is now structural, not procedural.
        // The lambda reads _tenantContext.TenantId at query execution time,
        // not at startup — so each scoped request gets the correct tenant.
        modelBuilder.Entity<Project>()
            .HasQueryFilter(p => p.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Domain.Entities.Task>()
            .HasQueryFilter(t => t.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.TenantId == _tenantContext.TenantId);

        base.OnModelCreating(modelBuilder);
    }


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Pass the custom class types as generic arguments
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableUtcDateTimeConverter>();
    }


}
// 1. Dedicated converter class for regular DateTime
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

// 2. Dedicated converter class for nullable DateTime?
public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter() : base(
        v => !v.HasValue ? v : (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)),
        v => !v.HasValue ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc))
    {
    }
}