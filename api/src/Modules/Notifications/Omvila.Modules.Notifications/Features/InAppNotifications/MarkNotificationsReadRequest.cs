namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme MarkNotificationsReadRequest: notificationIds veya all = true (tam biri).</summary>
internal sealed record MarkNotificationsReadRequest(IReadOnlyList<Guid>? NotificationIds, bool? All);
