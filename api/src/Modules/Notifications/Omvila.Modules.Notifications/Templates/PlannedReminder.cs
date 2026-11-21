using NodaTime;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>Planlanan hatırlatma: gönderim anı ve şablon çeşidi (null = "Yarın…", <see cref="SmsTemplates.SameDayVariant"/> = "Bugün…").</summary>
internal sealed record PlannedReminder(Instant ScheduledUtc, string? Variant);
