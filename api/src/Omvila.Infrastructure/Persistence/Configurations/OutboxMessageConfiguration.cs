using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Infrastructure.Messaging;

namespace Omvila.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_events");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Type).HasMaxLength(300);
        builder.Property(x => x.Payload).HasColumnType("jsonb");
        builder.Property(x => x.LastError).HasMaxLength(300);
        // Dağıtıcının taradığı küçük küme: işlenmemiş olaylar, deneme zamanına göre.
        builder.HasIndex(x => x.AvailableUtc).HasFilter("processed_utc IS NULL");
    }
}
