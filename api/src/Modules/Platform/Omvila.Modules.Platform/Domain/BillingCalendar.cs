using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// OMVILA faturalama takvimi (T-118): abonelik günleri (deneme bitişi, dönem, gecikme) şubeden bağımsız olarak OMVILA'nın saat
/// diliminde (<see cref="PlatformDefaults.DefaultTimeZoneId"/>) sayılır; ürün Türkiye pazarı içindir.
/// </summary>
internal static class BillingCalendar
{
    public static DateTimeZone Zone => DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];

    public static LocalDate Today(Instant now) => now.InZone(Zone).Date;

    /// <summary>Dönemin son günü: aylıkta bir ay, yıllıkta bir yıl sonrasından bir gün önce.</summary>
    public static LocalDate PeriodEnd(LocalDate start, BillingCycle cycle) =>
        (cycle == BillingCycle.Yearly ? start.PlusYears(1) : start.PlusMonths(1)).PlusDays(-1);
}
