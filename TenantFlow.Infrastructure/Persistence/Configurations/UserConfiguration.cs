using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo");
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Composite unique constraint — mirrors your UQ on (TenantId, Email) from Day 2
        // This ensures no two users in the same tenant share an email
        // but the same email CAN exist across different tenants
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique();

        // Relationship: many Users belong to one Tenant
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}