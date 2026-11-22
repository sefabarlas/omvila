using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>"BU HAFTA DOLULUK %74 · geçen hafta %68" ve "Bu hafta doluluk · personel bazında".</summary>
internal sealed record WeeklyOccupancy(
    LocalDate WeekStart,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? Percent,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? PreviousWeekPercent,
    IReadOnlyList<StaffOccupancy> Staff);
