using System.Text.Json.Serialization;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme MeNotificationPage.</summary>
/// <param name="Items">Bildirimler, yeniden eskiye.</param>
/// <param name="NextCursor">Sonraki sayfa imleci.</param>
/// <param name="UnreadCount">Okunmamış sayısı (sayfadan bağımsız).</param>
internal sealed record MeNotificationPage(
    IReadOnlyList<MeNotificationItem> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor,
    int UnreadCount);
