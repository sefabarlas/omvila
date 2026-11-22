using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>Sözleşme ManageOverview (salon paneli Genel bakış bento kartları ve haftalık doluluk). Yetkiye bağlı kartlar izin yoksa null.</summary>
internal sealed record ManageOverview(
    Guid BranchId,
    string BranchName,
    string TimeZone,
    LocalDate Date,
    Instant GeneratedUtc,
    Money TodayRevenue,
    int TodayAppointmentCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] OverviewPendingApproval? PendingApproval,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] OverviewWalkIns? WalkIns,
    WeeklyOccupancy WeeklyOccupancy);
