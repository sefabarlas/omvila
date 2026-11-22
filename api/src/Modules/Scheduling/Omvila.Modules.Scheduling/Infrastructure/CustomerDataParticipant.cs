using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Appointments;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// Müşteri birleştirme ve KVKK silmesinde Scheduling'in payı (T-92): randevular, tekrarlayan seriler, bekleme listesi
/// kayıtları ve kapıda sıra biletleri. Silmede bekleyen bekleme kaydı "Listeden çık" ile kapanır (teklif varsa reddedilir;
/// saat otomatik sıradakine verilmez), biletteki ad ve telefon kopyası anonimleşir.
/// </summary>
internal sealed class CustomerDataParticipant(SchedulingDbContext db, IOutbox outbox, IPackageSessions packageSessions) : ICustomerMergeParticipant, ICustomerErasureParticipant
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
        await db.WaitlistEntries
            .Where(w => w.BrandId == merge.BrandId && w.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(w => w.BrandCustomerId, merge.TargetBrandCustomerId).SetProperty(w => w.CustomerId, merge.TargetCustomerId), ct);
        await db.WalkInTickets
            .Where(t => t.BrandId == merge.BrandId && t.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.BrandCustomerId, (Guid?)merge.TargetBrandCustomerId), ct);
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

        await CloseWaitlistAsync(erasure, ct);
        await db.SaveChangesAsync(ct);
        await UnlinkAsync(erasure, ct);
        if (erasure.ByCustomer)
        {
            await AnnounceCustomerCancellationsAsync(upcoming, ct);
            return;
        }

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

    /// <summary>
    /// Müşterinin uygulamadan hesap silmesi (KK9; sözleşme meRequestAccountDeletion): seanslar geri döner ve iptal müşteri iptali olarak
    /// duyurulur (müşteriye mesaj gitmez; hatırlatmalar düşer, salona staff.appointment_changed, boşalan saat bekleme listesine).
    /// </summary>
    private async Task AnnounceCustomerCancellationsAsync(List<Appointment> cancelled, CancellationToken ct)
    {
        foreach (var brand in cancelled.GroupBy(a => a.BrandId))
        {
            await packageSessions.ReleaseAsync(brand.Key, brand, ct);
        }

        foreach (var appointment in cancelled)
        {
            await outbox.EnqueueAsync(
                new AppointmentCancelledByCustomer(
                    appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc,
                    nameof(CancellationReason.AccountDeleted)),
                ct);
        }
    }

    // Bekleyen bekleme kaydı "Listeden çık" ile kapanır (sözleşme manageRequestCustomerDeletion: "bekleme kayıtları kapanır").
    private async Task CloseWaitlistAsync(CustomerErasure erasure, CancellationToken ct)
    {
        var entries = await db.WaitlistEntries.Include(e => e.Offers)
            .Where(e => e.BrandId == erasure.BrandId && e.BrandCustomerId == erasure.BrandCustomerId && e.Status == Domain.Waitlist.WaitlistEntryStatus.Waiting)
            .ToListAsync(ct);
        foreach (var entry in entries)
        {
            entry.Leave(erasure.Now);
        }
    }

    // Global kişi bağı anonim kişiye döner; kapıda sıra biletindeki ad ve telefon kopyası silinir.
    private async Task UnlinkAsync(CustomerErasure erasure, CancellationToken ct)
    {
        await db.Appointments
            .Where(a => a.BrandId == erasure.BrandId && a.BrandCustomerId == erasure.BrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CustomerId, erasure.AnonymousCustomerId), ct);
        await db.AppointmentSeries
            .Where(a => a.BrandId == erasure.BrandId && a.BrandCustomerId == erasure.BrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.CustomerId, erasure.AnonymousCustomerId), ct);
        await db.WaitlistEntries
            .Where(w => w.BrandId == erasure.BrandId && w.BrandCustomerId == erasure.BrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(w => w.CustomerId, erasure.AnonymousCustomerId), ct);
        await db.WalkInTickets
            .Where(t => t.BrandId == erasure.BrandId && t.BrandCustomerId == erasure.BrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.CustomerName, AnonymizedName).SetProperty(t => t.PhoneE164, (string?)null), ct);
    }

    private const string AnonymizedName = "Silinmiş müşteri";
}
