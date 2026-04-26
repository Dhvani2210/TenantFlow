using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantFlow.Api.Entities;

namespace TenantFlow.Api.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        // Tell EF Core which table this entity maps to
        builder.ToTable("Tenants", "dbo");

        // Define the primary key
        builder.HasKey(t => t.TenantId);

        // Mirror the NEWSEQUENTIALID() default from your SQL schema
        // ValueGeneratedOnAdd tells EF Core the database generates this value on INSERT
        builder.Property(t => t.TenantId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        // Name column: required, max 100 chars — mirrors nvarchar(100) NOT NULL
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Email column: required, max 255 chars
        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255);

        // IsActive defaults to true on insert — mirrors DEFAULT 1
        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        // CreatedAt defaults to UTC now on insert — mirrors DEFAULT GETUTCDATE()
        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}