using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>"ONAY BEKLİYOR 2 · ilki 1 sa 35 dk içinde iptal olur".</summary>
internal sealed record OverviewPendingApproval(int Count, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? EarliestHoldExpiresUtc);
