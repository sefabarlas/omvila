using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Templates;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications;

/// <summary>
/// Bildirim paneli kayıtlarını yazar (sözleşme manageListInAppNotifications): kullanıcı başına bir satır, metin olayın push
/// şablonundan (müşteri adı "Ad Soyad"). Push tercihlerinden bağımsızdır. Kiracı bağlamı kurulmuş olmalı.
/// </summary>
internal sealed class InAppNotifier(NotificationsDbContext db, IClock clock, IIdGenerator ids)
{
    /// <summary>Hedef ekranlar (sözleşme NotificationTarget.type).</summary>
    public const string AppointmentTarget = "Appointment";

    public const string SmsCreditsTarget = "SmsCredits";

    public async Task AddAsync(InAppEntry entry, IReadOnlyCollection<Guid> userIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(userIds);
        if (!PushTemplates.TryRender(entry.EventCode, entry.Values, entry.Variant, out var title, out var body))
        {
            throw new InvalidOperationException($"Bildirim metni yok: {entry.EventCode} {entry.Variant}");
        }

        var now = clock.GetCurrentInstant();
        foreach (var userId in userIds.Distinct())
        {
            db.InAppNotifications.Add(InAppNotification.Create(
                ids.New(), entry.BrandId, userId, entry.EventCode, title, body, entry.BranchId, entry.TargetType, entry.TargetId, entry.HoldExpiresUtc, now));
        }

        await db.SaveChangesAsync(ct);
    }

    /// <summary>Okunmamış <paramref name="eventCode"/> bildirimi olan kullanıcılar (aynı uyarı tekrar yazılmaz).</summary>
    public async Task<IReadOnlySet<Guid>> UsersWithUnreadAsync(Guid brandId, string eventCode, IReadOnlyCollection<Guid> userIds, CancellationToken ct) =>
        (await db.InAppNotifications.AsNoTracking()
            .Where(n => n.BrandId == brandId && n.EventCode == eventCode && n.ReadUtc == null && userIds.Contains(n.UserId))
            .Select(n => n.UserId).Distinct().ToListAsync(ct)).ToHashSet();

    /// <summary>Talep yanıtlandı: randevunun bekleyen satırlarında kalan süre düşer (sözleşme: "Talep yanıtlandıysa null").</summary>
    public Task<int> ClearHoldsAsync(Guid brandId, Guid appointmentId, CancellationToken ct) =>
        db.InAppNotifications
            .Where(n => n.BrandId == brandId && n.TargetId == appointmentId && n.HoldExpiresUtc != null)
            .ExecuteUpdateAsync(set => set.SetProperty(n => n.HoldExpiresUtc, (Instant?)null), ct);
}
