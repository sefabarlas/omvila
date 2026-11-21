using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Reviews.IntegrationEventHandlers;

/// <summary>
/// A11 → değerlendirme daveti (durum-makinesi.md A11; T-88, T-89): tamamlanan randevu için daveti planlar. Kiracı bağlamı,
/// <c>ReviewInviteEnabled</c>, randevunun hâlâ tamamlanmış olması ve tek davet koşulları <see cref="IReviewInvitations"/> içinde denetlenir.
/// </summary>
internal sealed class ScheduleReviewInvite(IReviewInvitations invitations) : IIntegrationEventHandler<AppointmentCompleted>
{
    public async Task HandleAsync(AppointmentCompleted integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await invitations.ScheduleAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, integrationEvent.CompletedUtc, ct);
    }
}
