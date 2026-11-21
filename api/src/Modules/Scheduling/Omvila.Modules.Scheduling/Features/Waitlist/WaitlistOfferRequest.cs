using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme WaitlistOfferRequest ("Saat öner").</summary>
internal sealed record WaitlistOfferRequest(Instant? StartUtc, Guid? StaffId);
