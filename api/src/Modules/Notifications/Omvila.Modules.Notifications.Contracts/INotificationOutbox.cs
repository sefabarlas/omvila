namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Bildirimi iş transaction'ında notification_outbox'a yazar (bildirimler.md: kodda doğrudan SMS gönderilmez).
/// Gönderim DispatchNotifications işiyle yapılır; transaction geri alınırsa bildirim de yoktur.
/// </summary>
public interface INotificationOutbox
{
    /// <summary>Telefona SMS bildirimi kuyruğa ekler.</summary>
    Task EnqueueSmsAsync(SmsNotification notification, CancellationToken ct);

    /// <summary>
    /// Kayda bağlı, henüz gönderilmemiş (Pending) <paramref name="eventCode"/> satırlarını <c>Cancelled</c> yapar
    /// (bildirimler.md § İptal: randevu iptal olunca bekleyen hatırlatmalar düşer, saat değişince yeniden planlanır).
    /// Kiracı bağlamındaki markanın satırlarına uygulanır; düşürülen satır sayısını döner.
    /// </summary>
    Task<int> CancelPendingAsync(string eventCode, NotificationReference reference, CancellationToken ct);
}
