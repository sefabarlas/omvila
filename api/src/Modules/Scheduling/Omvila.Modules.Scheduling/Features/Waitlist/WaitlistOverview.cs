using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme WaitlistOverview.</summary>
internal sealed record WaitlistOverview(
    int WaitingCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? OldestJoinedUtc,
    int ConvertedThisMonth,
    int OfferHoldMinutes);
