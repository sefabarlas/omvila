using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>"ŞUBE 2 · 1 dahil + 1 ek şube".</summary>
internal sealed record BranchCounts(int Count, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? Included, int Extra);
