using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Infrastructure.Idempotency;

namespace Omvila.Infrastructure.Persistence.Configurations;

internal sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_keys");
        builder.HasKey(x => new { x.Scope, x.Key });
        builder.Property(x => x.Scope).HasMaxLength(100);
        builder.Property(x => x.RequestHash).HasMaxLength(64);
        builder.Property(x => x.Location).HasMaxLength(500);
        builder.HasIndex(x => x.CreatedUtc);
    }
}
