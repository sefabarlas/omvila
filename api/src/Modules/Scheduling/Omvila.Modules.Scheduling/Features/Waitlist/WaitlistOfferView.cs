using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme WaitlistOfferView.</summary>
internal sealed record WaitlistOfferView(
    Guid Id,
    string Status,
    Instant StartUtc,
    Instant EndUtc,
    Guid? StaffId,
    string? StaffName,
    Instant OfferedUtc,
    Instant HoldExpiresUtc);
