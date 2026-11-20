using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Omvila.Infrastructure.Messaging;

namespace Omvila.Infrastructure.Persistence.Configurations;

internal sealed class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.ToTable("processed_events");
        builder.HasKey(x => new { x.EventId, x.Handler });
        builder.Property(x => x.Handler).HasMaxLength(300);
    }
}
