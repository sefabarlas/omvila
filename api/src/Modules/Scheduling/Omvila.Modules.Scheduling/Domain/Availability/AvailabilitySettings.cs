namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Marka (ızgara için şube ezmesi uygulanmış) ayarları (sureler-ve-ayarlar.md): <c>MinNoticeMinutes</c>,
/// <c>MaxAdvanceDays</c> (hizmet değeri yoksa), <c>SlotIntervalMinutes</c> (şube ?? marka).
/// </summary>
internal sealed record AvailabilitySettings(int MinNoticeMinutes, int MaxAdvanceDays, int SlotIntervalMinutes);
