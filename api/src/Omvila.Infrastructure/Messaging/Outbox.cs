using System.Text.Json;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Messaging;

internal sealed class Outbox(InfraDbContext db, IClock clock, IIdGenerator ids) : IOutbox
{
    public async Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken ct)
        where TEvent : IIntegrationEvent
    {
        var now = clock.GetCurrentInstant();
        db.OutboxEvents.Add(new OutboxMessage
        {
            Id = ids.New(),
            Type = IntegrationEventRegistry.TypeName(typeof(TEvent)),
            Payload = JsonSerializer.Serialize(integrationEvent, IntegrationEventRegistry.Json),
            OccurredUtc = now,
            AvailableUtc = now,
        });
        await db.SaveChangesAsync(ct);
    }
}
