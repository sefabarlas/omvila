using System.Text.Json.Serialization;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme InAppNotificationPage (nextCursor zorunlu alan: son sayfada null yazılır).</summary>
internal sealed record InAppNotificationPage(
    IReadOnlyList<InAppNotificationItem> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor,
    int UnreadCount);
