namespace Omvila.Modules.Notifications.Domain;

/// <summary>alan-modeli.md § NotificationOutbox.Status.</summary>
internal enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Cancelled,
    Skipped,
}
