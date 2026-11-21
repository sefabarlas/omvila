using NodaTime;
using NodaTime.TimeZones;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// Sessiz saatler (sureler-ve-ayarlar.md QuietHoursLocal 22:00–08:00, şube yerel): bu aralığa düşen an aynı veya ertesi
/// sabah 08:00'e kayar. Müşteri hatırlatması ve işletme push'u kullanır; kod, kapora ve bekleme teklifi kaydırılmaz.
/// </summary>
internal static class QuietHours
{
    private static readonly LocalTime Start = new(PlatformDefaults.QuietHoursStartHour, 0);
    private static readonly LocalTime End = new(PlatformDefaults.QuietHoursEndHour, 0);

    public static Instant Defer(Instant at, DateTimeZone zone)
    {
        ArgumentNullException.ThrowIfNull(zone);
        var local = at.InZone(zone);
        var time = local.TimeOfDay;
        if (time >= Start)
        {
            return MorningOf(local.Date.PlusDays(1), zone);
        }

        return time < End ? MorningOf(local.Date, zone) : at;
    }

    private static Instant MorningOf(LocalDate date, DateTimeZone zone) =>
        zone.ResolveLocal(date.At(End), Resolvers.LenientResolver).ToInstant();
}
