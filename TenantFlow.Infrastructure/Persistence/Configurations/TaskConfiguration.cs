using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
    {
        builder.ToTable("Tasks", "dbo");
        builder.HasKey(t => t.TaskId);

        builder.Property(t => t.TaskId)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // Relationship: many Tasks belong to one Project
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: a Task is optionally assigned to one User
        // IsRequired(false) explicitly tells EF Core this FK is nullable
        // meaning a task can exist without an assigned user
        builder.HasOne(t => t.AssignedTo)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.TenantId)
            .IsRequired();

        // No navigation property on Task for Tenant — TenantId is purely for query filtering
        // HasOne<Tenant>() uses the type parameter directly instead of a lambda
        builder.HasOne<Tenant>()
            .WithMany()                          // Tenant has no Tasks collection navigation
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.Status)
            .HasConversion<string>();
    }
}