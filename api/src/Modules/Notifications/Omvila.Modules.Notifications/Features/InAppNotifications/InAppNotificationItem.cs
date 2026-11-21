using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme InAppNotification (readUtc zorunlu alan: okunmamışta null yazılır).</summary>
internal sealed record InAppNotificationItem(
    Guid Id,
    string Event,
    string Title,
    string Body,
    Instant CreatedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? ReadUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? BranchId,
    NotificationTarget Target)
{
    public static InAppNotificationItem From(InAppNotification n) => new(
        n.Id, n.EventCode, n.Title, n.Body, n.CreatedUtc, n.ReadUtc, n.BranchId, new NotificationTarget(n.TargetType, n.TargetId, n.HoldExpiresUtc));
}
