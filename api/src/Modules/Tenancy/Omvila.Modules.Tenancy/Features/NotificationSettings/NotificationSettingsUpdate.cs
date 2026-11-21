namespace Omvila.Modules.Tenancy.Features.NotificationSettings;

/// <summary>Sözleşme NotificationSettingsUpdate.</summary>
internal sealed record NotificationSettingsUpdate(
    IReadOnlyList<int>? ReminderOffsetsMinutes,
    bool? ReviewInviteEnabled,
    IReadOnlyList<NotificationTemplateUpdate>? Templates);
