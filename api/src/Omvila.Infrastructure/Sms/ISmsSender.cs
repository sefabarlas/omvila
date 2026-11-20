namespace Omvila.Infrastructure.Sms;

/// <summary>
/// SMS sağlayıcısı (AGENTS.md: dış servisler arayüz arkasında). Doğrudan çağrılmaz: bildirimler
/// notification_outbox'a yazılır, DispatchNotifications işi bu arayüzle gönderir (bildirimler.md).
/// </summary>
public interface ISmsSender
{
    Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct);
}
