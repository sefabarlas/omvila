using System.Text.Json.Serialization;

namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>"KAPIDA SIRADA 3 · ~35 dk bekleme".</summary>
internal sealed record OverviewWalkIns(int WaitingCount, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? EstimatedWaitMinutesForNew);
