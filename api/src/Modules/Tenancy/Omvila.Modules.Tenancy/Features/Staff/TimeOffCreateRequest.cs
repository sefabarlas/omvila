using NodaTime;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme TimeOffCreate.</summary>
internal sealed record TimeOffCreateRequest(
    string? Type, bool? IsAllDay, LocalDate? FromDate, LocalDate? ToDate, Instant? StartUtc, Instant? EndUtc, string? Note);
