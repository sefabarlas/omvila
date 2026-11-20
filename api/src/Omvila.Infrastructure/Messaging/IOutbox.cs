using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Messaging;

/// <summary>Olayı geçerli transaction'da infra.outbox_events'e yazar; commit olmazsa olay da yoktur (ADR-0005).</summary>
public interface IOutbox
{
    Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken ct)
        where TEvent : IIntegrationEvent;
}
