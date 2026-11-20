namespace Omvila.Modules.Tenancy.Features.Branches;

internal sealed record WeeklyHoursDayDto(int DayOfWeek, IReadOnlyList<TimeIntervalDto>? Intervals);
