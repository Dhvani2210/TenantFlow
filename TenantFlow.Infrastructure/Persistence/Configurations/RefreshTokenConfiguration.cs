using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantFlow.Domain.Entities;

namespace TenantFlow.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(r => r.RefreshTokenId);

        builder.Property(r => r.RefreshTokenId)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.TokenHash)
            .IsRequired()
            .HasMaxLength(512); // SHA-256 hash, base64-encoded

        builder.Property(r => r.ExpiresAt)
            .HasColumnType("timestamptz");

        builder.Property(r => r.CreatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.RevokedAt)
            .HasColumnType("timestamptz");

        // Fast lookup when validating an incoming refresh token
        builder.HasIndex(r => r.TokenHash);

        // No navigation property to User needed — UserId alone is enough,
        // same pattern as TenantId on Task
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade); // if a user is deleted, their tokens go too

    }
}