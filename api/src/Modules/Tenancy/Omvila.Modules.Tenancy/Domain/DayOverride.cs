using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Bir tarihin özel saati (alan-modeli.md § WorkingHoursOverride; tarih başına bir satır).</summary>
internal sealed record DayOverride(LocalDate Date, OverrideKind Kind, bool IsClosed, IReadOnlyList<OpeningInterval> Intervals, string? Note);
