using System.Text.Json.Serialization;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Sözleşme manageListCustomers yanıtı. nextCursor zorunludur: son sayfada açıkça null yazılır.</summary>
internal sealed record CustomerList(
    IReadOnlyList<CustomerSummaryResponse> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
