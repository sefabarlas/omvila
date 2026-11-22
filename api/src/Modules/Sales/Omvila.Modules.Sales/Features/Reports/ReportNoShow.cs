using System.Text.Json.Serialization;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>"GELMEDİ 5 · %4 · 2.150 ₺ kayıp".</summary>
internal sealed record ReportNoShow(int Count, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? Percent, Money LostRevenue);
