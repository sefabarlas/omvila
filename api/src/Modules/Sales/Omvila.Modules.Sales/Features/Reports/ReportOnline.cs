using System.Text.Json.Serialization;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>"ONLİNE PAY %46 · 54 randevu online".</summary>
internal sealed record ReportOnline(int AppointmentCount, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? Percent);
