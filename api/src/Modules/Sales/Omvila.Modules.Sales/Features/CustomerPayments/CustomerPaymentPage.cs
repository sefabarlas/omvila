using System.Text.Json.Serialization;
using Omvila.Modules.Sales.Features.Cash;

namespace Omvila.Modules.Sales.Features.CustomerPayments;

/// <summary>Sözleşme CustomerPaymentPage (müşteri kartı Ödemeler sekmesi).</summary>
internal sealed record CustomerPaymentPage(
    IReadOnlyList<CashPaymentView> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
