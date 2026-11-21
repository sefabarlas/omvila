using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Waitlist;
using Omvila.Modules.Scheduling.Infrastructure;

namespace Omvila.Modules.Scheduling.IntegrationEventHandlers;

/// <summary>
/// Randevu iptal edilince boşalan saat bekleme listesine teklif edilir (durum-makinesi.md A4–A7 "bekleme listesi
/// tetiklenir"; dilim-05 kabul kriteri 3): salon reddi (A4), süre dolumu (A5), müşteri iptali (A6) ve salon iptali (A7;
/// <c>offerToWaitlist</c> kapalıysa teklif yok). Randevunun her personeli için o personelin ilk kaleminin başlangıcı
/// boşalan saat sayılır (T-90). Olaydan sonra işlenir: kendi transaction'ında, markanın kiracı bağlamında; saat bu arada
/// başkasınca alındıysa motor teklif etmez.
/// </summary>
internal sealed class OfferVacatedSlotToWaitlist(SchedulingDbContext db, IDbSession session, WaitlistOfferEngine engine) :
    IIntegrationEventHandler<AppointmentDeclined>,
    IIntegrationEventHandler<AppointmentApprovalExpired>,
    IIntegrationEventHandler<AppointmentCancelledByCustomer>,
    IIntegrationEventHandler<AppointmentCancelledBySalon>
{
    public Task HandleAsync(AppointmentDeclined integrationEvent, CancellationToken ct) =>
        OfferAsync(integrationEvent?.BrandId ?? throw new ArgumentNullException(nameof(integrationEvent)), integrationEvent.AppointmentId, ct);

    public Task HandleAsync(AppointmentApprovalExpired integrationEvent, CancellationToken ct) =>
        OfferAsync(integrationEvent?.BrandId ?? throw new ArgumentNullException(nameof(integrationEvent)), integrationEvent.AppointmentId, ct);

    public Task HandleAsync(AppointmentCancelledByCustomer integrationEvent, CancellationToken ct) =>
        OfferAsync(integrationEvent?.BrandId ?? throw new ArgumentNullException(nameof(integrationEvent)), integrationEvent.AppointmentId, ct);

    public Task HandleAsync(AppointmentCancelledBySalon integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return integrationEvent.OfferToWaitlist ? OfferAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct) : Task.CompletedTask;
    }

    private async Task OfferAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var appointment = await db.Appointments.AsNoTracking()
            .Include(a => a.Items)
            .SingleOrDefaultAsync(a => a.Id == appointmentId && a.Status == AppointmentStatus.Cancelled, ct);
        if (appointment is null)
        {
            return;
        }

        var freed = appointment.Items
            .Where(i => i.StaffId is not null)
            .GroupBy(i => i.StaffId!.Value)
            .Select(g => (StaffId: g.Key, Start: g.Min(i => i.StartUtc)))
            .OrderBy(s => s.Start).ThenBy(s => s.StaffId);
        foreach (var (staffId, start) in freed)
        {
            await engine.OfferFreedSlotAsync(brandId, appointment.BranchId, staffId, start, ct);
        }
    }
}
