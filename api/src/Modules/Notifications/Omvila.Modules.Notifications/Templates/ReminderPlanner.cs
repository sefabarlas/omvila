using NodaTime;
using NodaTime.TimeZones;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// Hatırlatma zamanları (durum-makinesi.md § Sistem işleri ScheduleReminders; sureler-ve-ayarlar.md ReminderOffsetsMinutes,
/// QuietHoursLocal). Her ofset için başlangıç − ofset; sessiz saate (22:00–08:00 şube yerel) denk gelen 08:00'e kayar;
/// geçmişte kalan veya başlangıçtan sonraya düşen atlanır. Metin gönderim gününe göre seçilir: randevu günü "Bugün…",
/// önceki gün "Yarın…"; daha önceki gün için şablon olmadığından atlanır. Aynı metne iki hatırlatma düşerse (ör. 23:30
/// randevusunun 1 gün öncesi sessiz saatten randevu sabahına kayar) başlangıca yakın olan kalır (T-53).
/// </summary>
internal static class ReminderPlanner
{
    private static readonly LocalTime QuietStart = new(PlatformDefaults.QuietHoursStartHour, 0);
    private static readonly LocalTime QuietEnd = new(PlatformDefaults.QuietHoursEndHour, 0);

    public static IReadOnlyList<PlannedReminder> Plan(Instant startUtc, Instant now, DateTimeZone zone, IEnumerable<int> offsetsMinutes)
    {
        ArgumentNullException.ThrowIfNull(zone);
        ArgumentNullException.ThrowIfNull(offsetsMinutes);
        var appointmentDate = startUtc.InZone(zone).Date;
        var byVariant = new Dictionary<string, PlannedReminder>(StringComparer.Ordinal);
        foreach (var offset in offsetsMinutes.Where(o => o > 0).Distinct().OrderDescending())
        {
            var at = OutOfQuietHours(startUtc - Duration.FromMinutes(offset), zone);
            if (at <= now || at >= startUtc)
            {
                continue;
            }

            var sendDate = at.InZone(zone).Date;
            string? variant;
            if (sendDate == appointmentDate)
            {
                variant = SmsTemplates.SameDayVariant;
            }
            else if (sendDate == appointmentDate.PlusDays(-1))
            {
                variant = null;
            }
            else
            {
                continue;
            }

            byVariant[variant ?? string.Empty] = new PlannedReminder(at, variant);
        }

        return [.. byVariant.Values.OrderBy(r => r.ScheduledUtc)];
    }

    private static Instant OutOfQuietHours(Instant at, DateTimeZone zone)
    {
        var local = at.InZone(zone);
        var time = local.TimeOfDay;
        if (time >= QuietStart)
        {
            return MorningOf(local.Date.PlusDays(1), zone);
        }

        return time < QuietEnd ? MorningOf(local.Date, zone) : at;
    }

    private static Instant MorningOf(LocalDate date, DateTimeZone zone) =>
        zone.ResolveLocal(date.At(QuietEnd), Resolvers.LenientResolver).ToInstant();
}
