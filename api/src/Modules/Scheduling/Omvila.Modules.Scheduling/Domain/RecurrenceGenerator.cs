using NodaTime;
using NodaTime.TimeZones;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Tekrarlayan randevu üretimi (alan-modeli.md § AppointmentSeries; dilim-02 § Alan kuralları): haftalık 1/2/4, ilk
/// <see cref="PlatformDefaults.RecurringHorizonMonths"/> ay somut üretilir, çakışan tarih atlanır. Çakışma kararı
/// çağıranın verdiği yüklemdedir (müsaitlik motoru / yazma yolu). Yerel saat → an dönüşümü şube saat dilimiyle,
/// <see cref="Resolvers.LenientResolver"/> ile yapılır (musaitlik.md § Kavramlar).
/// </summary>
internal static class RecurrenceGenerator
{
    /// <summary>
    /// Ufuk sonu (hariç): başlangıç + 3 ay. "İlk 3 aylık randevular" başlangıçtan tam 3 ay sonraki günü kapsamaz (T-47).
    /// </summary>
    public static LocalDate HorizonEnd(LocalDate from) => from.PlusMonths(PlatformDefaults.RecurringHorizonMonths);

    /// <summary>
    /// <paramref name="firstDate"/>'ten başlayan haftalık tekrar tarihleri; <paramref name="afterExclusive"/> sonrası,
    /// <paramref name="untilExclusive"/> öncesi ve <paramref name="endsOn"/> dahil sınırında.
    /// </summary>
    public static IEnumerable<LocalDate> Dates(
        LocalDate firstDate, WeeklyRecurrence rule, LocalDate untilExclusive, LocalDate? endsOn = null, LocalDate? afterExclusive = null)
    {
        for (var date = firstDate; date < untilExclusive && (endsOn is null || date <= endsOn); date = date.PlusWeeks(rule.IntervalWeeks))
        {
            if (afterExclusive is null || date > afterExclusive)
            {
                yield return date;
            }
        }
    }

    /// <summary>
    /// Serinin ilk oluşturulması: ilk tarihten itibaren 3 ay. <paramref name="canBook"/> false dönen tekrar atlanır.
    /// İlk tekrar da yükleme sorulur; ilk tarihin çakışması çağıranın kararıdır (ör. oluşturmayı 409 ile reddetmek).
    /// </summary>
    public static RecurrencePlan Generate(
        LocalDate firstDate, LocalTime localStart, DateTimeZone zone, WeeklyRecurrence rule, LocalDate? endsOn, Func<Occurrence, bool> canBook) =>
        Extend(firstDate, localStart, zone, rule, endsOn, generatedUntil: null, HorizonEnd(firstDate), canBook);

    /// <summary>
    /// Ufku ileri kaydırma (ExtendRecurringSeries): <paramref name="generatedUntil"/> sonrasından
    /// <paramref name="untilExclusive"/> öncesine kadar.
    /// </summary>
    public static RecurrencePlan Extend(
        LocalDate firstDate,
        LocalTime localStart,
        DateTimeZone zone,
        WeeklyRecurrence rule,
        LocalDate? endsOn,
        LocalDate? generatedUntil,
        LocalDate untilExclusive,
        Func<Occurrence, bool> canBook)
    {
        ArgumentNullException.ThrowIfNull(zone);
        ArgumentNullException.ThrowIfNull(canBook);
        var planned = new List<Occurrence>();
        var skipped = new List<LocalDate>();
        foreach (var date in Dates(firstDate, rule, untilExclusive, endsOn, generatedUntil))
        {
            var occurrence = new Occurrence(date, StartOf(date, localStart, zone));
            if (canBook(occurrence))
            {
                planned.Add(occurrence);
            }
            else
            {
                skipped.Add(date);
            }
        }

        var lastConsidered = untilExclusive.PlusDays(-1);
        var until = endsOn is { } end && end < lastConsidered ? end : lastConsidered;
        return new RecurrencePlan(planned, skipped, generatedUntil is { } previous && previous > until ? previous : until);
    }

    public static Instant StartOf(LocalDate date, LocalTime localStart, DateTimeZone zone) =>
        zone.ResolveLocal(date.At(localStart), Resolvers.LenientResolver).ToInstant();

    /// <summary>
    /// TekrarIptal: "Yalnızca bu randevu" yalnız seçilen tarihi, "Bu ve sonraki randevular" seçilen ve sonraki tarihleri
    /// etkiler; öncekiler durur (dilim-02 kabul kriteri 13).
    /// </summary>
    public static IEnumerable<T> Affected<T>(IEnumerable<T> occurrences, Func<T, LocalDate> dateOf, LocalDate selected, SeriesScope scope)
    {
        ArgumentNullException.ThrowIfNull(occurrences);
        ArgumentNullException.ThrowIfNull(dateOf);
        return scope == SeriesScope.ThisOnly
            ? occurrences.Where(o => dateOf(o) == selected)
            : occurrences.Where(o => dateOf(o) >= selected);
    }
}
