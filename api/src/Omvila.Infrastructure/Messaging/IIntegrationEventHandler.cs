using System.Diagnostics.CodeAnalysis;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Messaging;

/// <summary>
/// Entegrasyon olayı işleyicisi. Kendi transaction'ında çalışır; kiracı bağlamını olaydaki BrandId'den kurar
/// (IDbSession.SetTenantAsync). Aynı olay için ikinci kez çağrılmaz (infra.processed_events).
/// </summary>
[SuppressMessage("Naming", "CA1711", Justification = ".NET olay temsilcisi değil; ad mimari belgedeki IntegrationEventHandlers ile aynı.")]
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent, CancellationToken ct);
}
