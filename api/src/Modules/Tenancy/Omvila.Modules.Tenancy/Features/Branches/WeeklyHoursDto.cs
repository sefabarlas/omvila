namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme WeeklyHours.</summary>
internal sealed record WeeklyHoursDto(IReadOnlyList<WeeklyHoursDayDto>? Days, string? Version = null);
