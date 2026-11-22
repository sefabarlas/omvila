using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>Sözleşme MyCommissionSummary (işletme uygulaması Profil, "Primim").</summary>
internal sealed record MyCommissionSummary(
    Guid StaffId,
    string StaffName,
    string TimeZone,
    string Month,
    LocalDate From,
    LocalDate To,
    long TotalMinor,
    long ServiceRevenueMinor,
    string Currency,
    int AppointmentCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? NoShowPercent,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? AverageRating,
    IReadOnlyList<CommissionDayAmount> Daily,
    IReadOnlyList<CommissionWeekDay> ThisWeek,
    IReadOnlyList<MyCommissionEntry> Entries,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] NextAccrual? NextAccrual);
