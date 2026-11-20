using NodaTime;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme WorkingHoursOverride.</summary>
internal sealed record OverrideDto(Guid Id, LocalDate Date, string Kind, bool IsClosed, IReadOnlyList<TimeIntervalDto> Intervals, string? Note);
