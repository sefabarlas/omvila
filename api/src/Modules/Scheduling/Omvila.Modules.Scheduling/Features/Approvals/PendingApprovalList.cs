using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>
/// Sözleşme PendingApprovalList ("Dikkat isteyenler", "Onay bekliyor" bentosu). <see cref="EarliestHoldExpiresUtc"/>
/// zorunlu ve null olabilir (liste boşsa); boş alan yazmama kuralına istisna.
/// </summary>
internal sealed record PendingApprovalList(
    Instant GeneratedUtc,
    int TotalCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? EarliestHoldExpiresUtc,
    IReadOnlyList<PendingApprovalItem> Items);
