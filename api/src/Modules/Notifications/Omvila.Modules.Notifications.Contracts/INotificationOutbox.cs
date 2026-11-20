namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Bildirimi iş transaction'ında notification_outbox'a yazar (bildirimler.md: kodda doğrudan SMS gönderilmez).
/// Gönderim DispatchNotifications işiyle yapılır; transaction geri alınırsa bildirim de yoktur.
/// </summary>
public interface INotificationOutbox
{
    /// <summary>Telefona SMS bildirimi kuyruğa ekler.</summary>
    Task EnqueueSmsAsync(SmsNotification notification, CancellationToken ct);
}
