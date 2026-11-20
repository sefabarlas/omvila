using NodaTime;

namespace Omvila.Infrastructure.Messaging;

/// <summary>infra.processed_events: (event_id, handler) işlendi; işleyici aynı olayı ikinci kez çalıştırmaz.</summary>
internal sealed class ProcessedEvent
{
    public Guid EventId { get; init; }

    public required string Handler { get; init; }

    public Instant ProcessedUtc { get; init; }
}
