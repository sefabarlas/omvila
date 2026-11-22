using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme SubscriptionInvoicePage.</summary>
internal sealed record SubscriptionInvoicePage(
    IReadOnlyList<SubscriptionInvoiceView> Items, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
