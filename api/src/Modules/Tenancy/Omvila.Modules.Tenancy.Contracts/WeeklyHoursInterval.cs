using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Haftalık saat aralığı (yerel, [Start, End)).</summary>
/// <param name="Day">Gün.</param>
/// <param name="Start">Başlangıç.</param>
/// <param name="End">Bitiş.</param>
public sealed record WeeklyHoursInterval(IsoDayOfWeek Day, LocalTime Start, LocalTime End);
