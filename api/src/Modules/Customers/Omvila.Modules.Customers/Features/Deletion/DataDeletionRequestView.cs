using NodaTime;

namespace Omvila.Modules.Customers.Features.Deletion;

/// <summary>Sözleşme DataDeletionRequestView.</summary>
internal sealed record DataDeletionRequestView(
    Guid Id,
    Guid? BrandCustomerId,
    string CustomerDisplayName,
    string RequestedVia,
    Instant RequestedUtc,
    LocalDate DueOn,
    string Status,
    Instant? CompletedUtc);
