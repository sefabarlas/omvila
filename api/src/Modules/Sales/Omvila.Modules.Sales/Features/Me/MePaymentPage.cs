using System.Text.Json.Serialization;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Sözleşme MePaymentPage.</summary>
internal sealed record MePaymentPage(
    IReadOnlyList<MePaymentRow> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
