using System.Text.Json.Serialization;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>Sözleşme AdminCreditTransactionPage, SubscriptionInvoicePage ve AdminPaymentNoticePage biçimi (items + nextCursor).</summary>
internal sealed record AdminPage<T>(IReadOnlyList<T> Items, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
