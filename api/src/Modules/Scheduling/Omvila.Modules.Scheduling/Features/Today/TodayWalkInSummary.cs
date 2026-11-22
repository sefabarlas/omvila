using System.Text.Json.Serialization;

namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>Sözleşme TodayWalkInSummary ("Sırada 3 · ~35 dk bekleme").</summary>
internal sealed record TodayWalkInSummary(
    int WaitingCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? EstimatedWaitMinutesForNew);
