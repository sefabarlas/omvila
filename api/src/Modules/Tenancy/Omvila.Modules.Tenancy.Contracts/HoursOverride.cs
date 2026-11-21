using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Özel gün (WorkingHoursOverride): kapalı gün veya o güne özel saatler ("Yalnızca bu gün için aç").</summary>
/// <param name="Date">Yerel gün.</param>
/// <param name="IsClosed">Kapalı.</param>
/// <param name="Intervals">Açık saatler (kapalıysa boş).</param>
public sealed record HoursOverride(LocalDate Date, bool IsClosed, IReadOnlyList<LocalTimeInterval> Intervals);
