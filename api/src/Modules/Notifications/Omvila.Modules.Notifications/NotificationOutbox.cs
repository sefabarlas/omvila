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
            notification.Variant));
        await db.SaveChangesAsync(ct);
    }

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
