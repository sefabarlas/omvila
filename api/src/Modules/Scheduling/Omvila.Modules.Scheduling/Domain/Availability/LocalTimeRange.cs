using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Bir gün içindeki yerel saat aralığı <c>[Start, End)</c>; gece yarısını aşmaz (alan-modeli.md § BranchWorkingHours).</summary>
internal readonly record struct LocalTimeRange(LocalTime Start, LocalTime End);
