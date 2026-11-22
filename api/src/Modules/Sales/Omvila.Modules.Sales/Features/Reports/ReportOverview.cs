using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>Sözleşme ReportOverview ("Raporlar ve prim" üst kartları ve günlük tahsilat).</summary>
internal sealed record ReportOverview(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? BranchId,
    string TimeZone,
    LocalDate From,
    LocalDate To,
    Money EarnedRevenue,
    int CompletedAppointmentCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? EarnedRevenueChangePercent,
    Money Collections,
    ReportNoShow NoShow,
    ReportOnline Online,
    IReadOnlyList<ReportDailyCollection> DailyCollections,
    string Currency);
