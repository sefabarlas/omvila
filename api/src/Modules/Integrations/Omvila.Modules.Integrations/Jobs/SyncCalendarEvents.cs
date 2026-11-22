using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Integrations.Calendar;
using Omvila.Modules.Integrations.Domain;
using Omvila.Modules.Integrations.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Integrations.Jobs;

/// <summary>
/// SyncCalendarEvents (mimari.md: Integrations, 5 dk; K7). Tek yönlü: her etkin bağlantı için personelin pencere içindeki
/// randevuları (dünden SyncWindowDays sonrasına) takvimle uzlaştırılır: yeni randevu → olay, saat veya hizmet değişti → olay
/// güncellenir, iptal veya personelden alındı → olay silinir. Olay başlığı "Hizmetler · Ad S."; müşterinin telefonu ve notu yazılmaz.
/// Sağlayıcı erişimi reddederse bağlantı TokenExpired olur; geçici hatada kayıt Failed kalır ve sonraki turda yeniden yazılır.
/// Olay işleyicisi yerine uzlaştırma seçildi: randevunun her değişiklik yolu (taşıma, personel değişimi, iptal türleri) için ayrı
/// olay gerekmez, gecikme en fazla bir tur (T-121).
/// </summary>
internal sealed partial class SyncCalendarEvents(
    IntegrationsDbContext db,
    IDbSession session,
    ICalendarProvider provider,
    CalendarTokenProtector protector,
    IAppointmentQueries appointments,
    ICustomerDirectory customers,
    IIdGenerator ids,
    IOptions<CalendarOptions> options,
    IClock clock,
    ILogger<SyncCalendarEvents> logger) : IPeriodicJob
{
    public string Key => "SyncCalendarEvents";

    public async Task RunAsync(CancellationToken ct)
    {
        if (!provider.IsEnabled)
        {
            return;
        }

        var now = clock.GetCurrentInstant();
        var links = await db.CalendarLinks.AsNoTracking()
            .Where(l => l.Status == CalendarLinkStatus.Active && l.TokenExpiresUtc > now)
            .Select(l => new { l.Id, l.BrandId })
            .ToListAsync(ct);
        foreach (var item in links)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(item.BrandId), ct);
            var link = await db.CalendarLinks.SingleAsync(l => l.Id == item.Id, ct);
            await SyncLinkAsync(link, now, ct);
            await db.SaveChangesAsync(ct);
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    private async Task SyncLinkAsync(CalendarLink link, Instant now, CancellationToken ct)
    {
        var from = now - Duration.FromDays(1);
        var to = now + Duration.FromDays(options.Value.SyncWindowDays);
        var rows = await appointments.ListForStaffCalendarAsync(link.BrandId, link.StaffId, from, to, ct);
        var names = await customers.GetDisplayInfoAsync(link.BrandId, [.. rows.Select(r => r.BrandCustomerId).Distinct()], ct);
        var syncs = await db.CalendarSyncs.Where(s => s.CalendarLinkId == link.Id).ToDictionaryAsync(s => s.AppointmentId, ct);
        var access = new CalendarAccess(protector.Unprotect(link.Id, link.AccessTokenEncrypted!), link.ExternalCalendarId);
        var seen = new HashSet<Guid>();
        foreach (var row in rows)
        {
            seen.Add(row.AppointmentId);
            syncs.TryGetValue(row.AppointmentId, out var sync);
            var status = row.IsCancelled
                ? await DeleteAsync(access, sync, now, ct)
                : await UpsertAsync(link, access, sync, row, names.TryGetValue(row.BrandCustomerId, out var c) ? c.FullName : null, now, ct);
            if (status == CalendarCallStatus.Revoked)
            {
                link.MarkTokenExpired(now);
                LogDisconnected(logger, link.Id, link.BrandId);
                return;
            }
        }

        // Personelden alınan veya pencereden düşmeden kaybolan randevunun olayı silinir.
        foreach (var orphan in syncs.Values.Where(s => !seen.Contains(s.AppointmentId) && s.SyncStatus == CalendarSyncStatus.Synced && s.EventStartUtc >= from))
        {
            if (await DeleteAsync(access, orphan, now, ct) == CalendarCallStatus.Revoked)
            {
                link.MarkTokenExpired(now);
                return;
            }
        }

        link.MarkSynced(now);
    }

    private async Task<CalendarCallStatus> UpsertAsync(
        CalendarLink link, CalendarAccess access, AppointmentCalendarSync? sync, StaffCalendarAppointment row, string? customerName, Instant now, CancellationToken ct)
    {
        var data = new CalendarEventData(Title(row.ServiceNames, customerName), "OMVILA randevusu", row.StartUtc, row.EndUtc);
        var hash = Hash(data);
        if (sync is not null && !sync.NeedsWrite(hash))
        {
            return CalendarCallStatus.Ok;
        }

        if (sync is null)
        {
            sync = AppointmentCalendarSync.Create(ids.New(), link.BrandId, link.Id, row.AppointmentId);
            db.CalendarSyncs.Add(sync);
        }

        var eventId = sync.SyncStatus == CalendarSyncStatus.Deleted ? null : sync.ExternalEventId;
        var result = await provider.UpsertEventAsync(access, eventId, data, ct);
        if (result.Status == CalendarCallStatus.Ok)
        {
            sync.Synced(result.EventId!, hash, row.StartUtc, now);
        }
        else
        {
            sync.Failed(now);
        }

        return result.Status;
    }

    private async Task<CalendarCallStatus> DeleteAsync(CalendarAccess access, AppointmentCalendarSync? sync, Instant now, CancellationToken ct)
    {
        if (sync is null || sync.SyncStatus == CalendarSyncStatus.Deleted || sync.ExternalEventId is null)
        {
            return CalendarCallStatus.Ok;
        }

        var result = await provider.DeleteEventAsync(access, sync.ExternalEventId, ct);
        if (result.Status == CalendarCallStatus.Ok)
        {
            sync.Deleted(now);
        }

        return result.Status;
    }

    // "Saç kesimi + Fön · Selin Ö." (soyadın baş harfi; panel ve push'taki kısaltmayla aynı).
    internal static string Title(IReadOnlyList<string> services, string? customerName)
    {
        var title = string.Join(" + ", services);
        if (string.IsNullOrWhiteSpace(customerName))
        {
            return title;
        }

        var parts = customerName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var shortName = parts.Length > 1 ? $"{string.Join(' ', parts[..^1])} {parts[^1][0]}." : parts[0];
        return $"{title} · {shortName}";
    }

    private static string Hash(CalendarEventData data) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{data.Title}|{data.Description}|{data.StartUtc}|{data.EndUtc}")));

    [LoggerMessage(Level = LogLevel.Warning, Message = "Calendar link {LinkId} of brand {BrandId} lost access during sync (token expired)")]
    private static partial void LogDisconnected(ILogger logger, Guid linkId, Guid brandId);
}
