using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>Sözleşme ManageToday (işletme uygulaması "Bugün"). Yetkiye bağlı kartlar izin yoksa null yazılır.</summary>
internal sealed record ManageToday(
    Guid BranchId,
    string BranchName,
    string TimeZone,
    LocalDate Date,
    Instant GeneratedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] TodayAppointment? Next,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? PendingApprovalCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] TodayWalkInSummary? WalkIns,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] TodayCommission? Commission,
    int CompletedCount,
    int TotalCount,
    IReadOnlyList<TodayAppointment> Items);
