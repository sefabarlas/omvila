using System.Text.Json.Serialization;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>Sözleşme CommissionReportRow (Personel, Randevu, Hizmet cirosu, Oran, Prim).</summary>
internal sealed record CommissionReportRow(
    Guid StaffId,
    string StaffName,
    int AppointmentCount,
    long ServiceRevenueMinor,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? ServiceCommissionPercent,
    long ServiceCommissionMinor,
    long SaleCommissionMinor,
    long TotalCommissionMinor);
