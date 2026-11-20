using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Bir tekrarın yerel tarihi ve şube saat dilimine göre başlangıç anı.</summary>
internal sealed record Occurrence(LocalDate Date, Instant StartUtc);
