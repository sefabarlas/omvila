using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>Sözleşme CommissionReport ("Personel ve prim").</summary>
internal sealed record CommissionReport(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? BranchId,
    LocalDate From,
    LocalDate To,
    string Currency,
    IReadOnlyList<CommissionReportRow> Items,
    long TotalCommissionMinor);
