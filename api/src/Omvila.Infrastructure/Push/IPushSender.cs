namespace Omvila.Infrastructure.Push;

/// <summary>
/// Push sağlayıcısı (AGENTS.md: dış servisler arayüz arkasında). Doğrudan çağrılmaz: bildirimler notification_outbox'a
/// yazılır, DispatchNotifications işi bu arayüzle gönderir (bildirimler.md § Outbox).
/// </summary>
public interface IPushSender
{
    Task<PushSendResult> SendAsync(PushMessage message, CancellationToken ct);
}
