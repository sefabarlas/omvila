using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Jobs;

/// <summary>
/// Gün sonu özeti (bildirimler.md staff.daily_summary; sureler-ve-ayarlar.md DailySummaryLocalTime 20:00, şube yerel; T-105 (6),
/// T-120). Şubenin yerel saati 20:00'yi geçtiyse ve o gün için özet yazılmadıysa: bugün başlayıp tamamlanan randevu sayısı ve
/// bugünün net tahsilatı (Sales.Contracts <see cref="ICollectionQueries"/>) <see cref="BranchDailySummary"/> satırına yazılır ve
/// <see cref="BranchDailySummaryReady"/> yayımlanır (alıcı ve tercih süzgeci Notifications'ta). Aday şubeler son bir günde
/// randevu tamamlamış şubelerdir; tamamlanan randevusu ve tahsilatı olmayan gün için özet yazılmaz. 20:00 ile gece yarısı
/// arasında iş hiç çalışmazsa o günün özeti ertesi gün gönderilmez (eski özet gecikmeli gitmez).
/// </summary>
internal sealed class SendDailySummaries(
    SchedulingDbContext db, IDbSession session, ITenantQueries tenants, ICollectionQueries collections, IOutbox outbox, IClock clock) : IPeriodicJob
{
    private static readonly LocalTime SummaryLocalTime = new(PlatformDefaults.DailySummaryHour, 0);

    public string Key => "SendDailySummaries";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var since = now - Duration.FromDays(1);
        var candidates = await db.Appointments.AsNoTracking()
            .Where(a => a.Status == AppointmentStatus.Completed && a.CompletedUtc != null && a.CompletedUtc >= since)
            .Select(a => new { a.BrandId, a.BranchId })
            .Distinct()
            .ToListAsync(ct);

        foreach (var brand in candidates.GroupBy(c => c.BrandId))
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brand.Key), ct);
            foreach (var candidate in brand)
            {
                await SummarizeAsync(brand.Key, candidate.BranchId, now, ct);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    // Kural: bildirimler.md staff.daily_summary ("{n} randevu tamamlandı, {tutar} tahsilat."); sureler-ve-ayarlar.md DailySummaryLocalTime
    private async Task SummarizeAsync(Guid brandId, Guid branchId, Instant now, CancellationToken ct)
    {
        if (await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) is not { } zoneId || DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) is not { } zone)
        {
            return;
        }

        var local = now.InZone(zone);
        if (local.TimeOfDay < SummaryLocalTime
            || await db.BranchDailySummaries.AnyAsync(s => s.BrandId == brandId && s.BranchId == branchId && s.BusinessDate == local.Date, ct))
        {
            return;
        }

        var dayStart = zone.AtStartOfDay(local.Date).ToInstant();
        var dayEnd = zone.AtStartOfDay(local.Date.PlusDays(1)).ToInstant();
        var completed = await db.Appointments.CountAsync(
            a => a.BranchId == branchId && a.Status == AppointmentStatus.Completed && a.StartUtc >= dayStart && a.StartUtc < dayEnd, ct);
        var collected = await collections.GetNetCollectedAsync(brandId, branchId, dayStart, dayEnd, ct);
        if (completed == 0 && collected == 0)
        {
            return;
        }

        db.BranchDailySummaries.Add(BranchDailySummary.Record(brandId, branchId, local.Date, completed, collected, now));
        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(new BranchDailySummaryReady(brandId, branchId, zone.Id, completed, collected), ct);
    }
}
