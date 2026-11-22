using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Templates;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications;

internal sealed class NotificationOutbox(NotificationsDbContext db, IClock clock, IIdGenerator ids) : INotificationOutbox
{
    public async Task EnqueueSmsAsync(SmsNotification notification, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(notification);
        if (!PhoneNumber.TryParse(notification.PhoneE164, out var phone))
        {
            throw new DomainException("Bildirim alıcısı geçerli bir telefon değil.");
        }

        // Şablon ve değişkenler kuyruğa yazarken denetlenir: eksik değişken gönderimde değil burada patlar.
        _ = SmsTemplates.Render(notification.EventCode, notification.Values, notification.Variant);

        db.Outbox.Add(NotificationMessage.ForPhone(
            ids.New(),
            notification.BrandId,
            notification.EventCode,
            phone.E164,
            JsonSerializer.Serialize(notification.Values),
            clock.GetCurrentInstant(),
            notification.ScheduledUtc,
            notification.Reference?.Type,
            notification.Reference?.Id,
            notification.Variant,
            PushCustomer(notification)));
        await db.SaveChangesAsync(ct);
    }

    public async Task EnqueueStaffPushAsync(StaffPushNotification notification, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(notification);
        if (!PushTemplates.TryRender(notification.EventCode, notification.Values, notification.Variant, out _, out _))
        {
            throw new InvalidOperationException($"Push şablonu yok: {notification.EventCode} {notification.Variant}");
        }

        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(notification.TimeZoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var now = clock.GetCurrentInstant();

        // Kural: bildirimler.md § Sessiz saatler — işletme push'u sessiz saatte gönderilmez, sabaha ertelenir.
        var scheduled = QuietHours.Defer(now, zone);
        var payload = JsonSerializer.Serialize(notification.Values);
        foreach (var userId in notification.UserIds.Distinct())
        {
            db.Outbox.Add(NotificationMessage.ForUser(
                ids.New(), notification.BrandId, notification.EventCode, userId, payload, now, scheduled,
                notification.Reference?.Type, notification.Reference?.Id, notification.Variant));
        }

        await db.SaveChangesAsync(ct);
    }

    // Kural: bildirimler.md § Kanal planı — push metni olan müşteri mesajı [Push, Sms]; giriş kodu ve kapora mesajları
    // (deposit.*: IBAN ve tutar içerir, V3) her zaman SMS.
    private static Guid? PushCustomer(SmsNotification notification) =>
        notification.CustomerId is { } customerId
        && !string.Equals(notification.EventCode, NotificationEvents.OtpCode, StringComparison.Ordinal)
        && !notification.EventCode.StartsWith(DepositEventPrefix, StringComparison.Ordinal)
        && PushTemplates.Has(notification.EventCode, notification.Variant)
            ? customerId
            : null;

    private const string DepositEventPrefix = "deposit.";

    public Task<int> CancelPendingAsync(string eventCode, NotificationReference reference, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventCode);
        ArgumentNullException.ThrowIfNull(reference);
        return db.Outbox
            .Where(m => m.Status == NotificationStatus.Pending && m.EventCode == eventCode
                && m.ReferenceType == reference.Type && m.ReferenceId == reference.Id)
            .ExecuteUpdateAsync(set => set.SetProperty(m => m.Status, NotificationStatus.Cancelled), ct);
    }
}
