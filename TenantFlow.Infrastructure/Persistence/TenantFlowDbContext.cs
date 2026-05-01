using Microsoft.EntityFrameworkCore;
using TenantFlow.Persistence.Configurations;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Infrastructure.Persistence;

public class TenantFlowDbContext : DbContext
{
    // The constructor receives options (connection string, provider, etc.)
    // and passes them up to the base DbContext class
    public TenantFlowDbContext(DbContextOptions<TenantFlowDbContext> options)
        : base(options) { }

    // One DbSet per entity — these are your C# "tables"
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Domain.Entities.Task> Tasks => Set<Domain.Entities.Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This single line finds every IEntityTypeConfiguration class
        // in this assembly and applies them all automatically
        // So as you add more entities later, you just add a config class
        // and this line picks it up — no changes needed here
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}