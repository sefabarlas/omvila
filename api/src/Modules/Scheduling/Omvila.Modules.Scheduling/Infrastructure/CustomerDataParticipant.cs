using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// Müşteri birleştirme ve KVKK silmesinde Scheduling'in payı (T-92): randevular ve tekrarlayan seriler. Bekleme listesi
/// kayıtları bekleme modülüyle birlikte eklenecek.
/// </summary>
internal sealed class CustomerDataParticipant(SchedulingDbContext db, IOutbox outbox) : ICustomerMergeParticipant, ICustomerErasureParticipant
{
    private static readonly AppointmentStatus[] Upcoming = [AppointmentStatus.Draft, AppointmentStatus.PendingApproval, AppointmentStatus.Confirmed];

    // Kural: alan-modeli.md § Birleştirme (randevular hedefe taşınır)
    public async Task MergeAsync(CustomerMerge merge, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(merge);
        await db.Appointments
            .Where(a => a.BrandId == merge.BrandId && a.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.BrandCustomerId, merge.TargetBrandCustomerId).SetProperty(a => a.CustomerId, merge.TargetCustomerId), ct);
        await db.AppointmentSeries
            .Where(a => a.BrandId == merge.BrandId && a.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.BrandCustomerId, merge.TargetBrandCustomerId).SetProperty(a => a.CustomerId, merge.TargetCustomerId), ct);
    }

    public async Task<CustomerErasureFootprint> GetFootprintAsync(Guid brandId, Guid brandCustomerId, NodaTime.Instant now, CancellationToken ct)
    {
        var appointments = db.Appointments.AsNoTracking().Where(a => a.BrandId == brandId && a.BrandCustomerId == brandCustomerId);
        // Taslak (online tutma) randevu sayılmaz; iptal edilen yaklaşan randevular da anonim kayıt olarak kalır.
        var kept = appointments.Where(a => a.Status != AppointmentStatus.Draft);
        var total = await kept.CountAsync(ct);
        var upcoming = await kept.CountAsync(a => Upcoming.Contains(a.Status) && a.StartUtc > now, ct);
        return new CustomerErasureFootprint(Appointments: total, UpcomingAppointments: upcoming);
    }

    // Kural: alan-modeli.md § DataDeletionRequest (yaklaşan randevular iptal, neden AccountDeleted; randevu kayıtları anonim kalır)
    public async Task EraseAsync(CustomerErasure erasure, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(erasure);
        var upcoming = await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => a.BrandId == erasure.BrandId && a.BrandCustomerId == erasure.BrandCustomerId && Upcoming.Contains(a.Status) && a.StartUtc > erasure.Now)
            .ToListAsync(ct);
        foreach (var appointment in upcoming)
        {
            appointment.CancelForAccountDeletion(erasure.Now, erasure.UserId);
        }

        var series = await db.AppointmentSeries
            .Where(s => s.BrandId == erasure.BrandId && s.BrandCustomerId == erasure.BrandCustomerId && s.Status != SeriesStatus.Ended)
            .ToListAsync(ct);
        foreach (var s in series)
        {
            s.End(erasure.Now);
        }

        await db.SaveChangesAsync(ct);
        await db.Appointments
            .Where(a => a.BrandId == erasure.BrandId && a.BrandCustomerId == erasure.BrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CustomerId, erasure.AnonymousCustomerId), ct);
        await db.AppointmentSeries
            .Where(a => a.BrandId == erasure.BrandId && a.BrandCustomerId == erasure.BrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CustomerId, erasure.AnonymousCustomerId), ct);

        // Bildirim gitmez (NotifyCustomer = false); Notifications planlanmış hatırlatmaları düşürür (T-53).
        foreach (var appointment in upcoming)
        {
            await outbox.EnqueueAsync(
                new AppointmentCancelledBySalon(
                    appointment.BrandId, appointment.Id, appointment.BrandCustomerId, appointment.StartUtc,
                    nameof(CancellationReason.AccountDeleted), NotifyCustomer: false),
                ct);
        }
    }
}
