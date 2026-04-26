using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantFlow.Api.Entities;

namespace TenantFlow.Api.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects", "dbo");
        builder.HasKey(p => p.ProjectId);

        builder.Property(p => p.ProjectId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Description is nullable — no IsRequired(), mirrors NULL allowed
        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationship: many Projects belong to one Tenant
        // HasOne says "a Project has one Tenant"
        // WithMany says "that Tenant has many Projects"
        // HasForeignKey says "the column that holds this link is TenantId"
        // OnDelete Restrict mirrors your ON DELETE NO ACTION from Day 2
        builder.HasOne(p => p.Tenant)
            .WithMany(t => t.Projects)
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}