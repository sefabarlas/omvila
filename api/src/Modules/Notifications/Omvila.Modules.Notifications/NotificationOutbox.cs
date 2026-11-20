using System.Text.Json;
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
        _ = SmsTemplates.Render(notification.EventCode, notification.Values);

        db.Outbox.Add(NotificationMessage.ForPhone(
            ids.New(),
            notification.BrandId,
            notification.EventCode,
            phone.E164,
            JsonSerializer.Serialize(notification.Values),
            clock.GetCurrentInstant()));
        await db.SaveChangesAsync(ct);
    }
}
