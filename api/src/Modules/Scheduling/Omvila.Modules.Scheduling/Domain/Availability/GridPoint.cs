using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Izgaradaki bir başlangıç: yerel saat ve karşılığı olan an.</summary>
internal readonly record struct GridPoint(LocalTime Local, Instant Instant);
