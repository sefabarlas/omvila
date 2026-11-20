using NodaTime;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme WorkingHoursOverrideCreate.</summary>
internal sealed record OverrideCreateRequest(
    LocalDate? FromDate, LocalDate? ToDate, string? Kind, bool? IsClosed, IReadOnlyList<TimeIntervalDto>? Intervals, string? Note);
