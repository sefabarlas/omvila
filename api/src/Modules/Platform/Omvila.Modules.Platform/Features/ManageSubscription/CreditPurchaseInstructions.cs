using System.Text.Json.Serialization;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme CreditPurchaseInstructions.</summary>
internal sealed record CreditPurchaseInstructions(
    Guid CreditPackageId,
    int Credits,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? Amount,
    string TransferCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] OmvilaBankAccountView? OmvilaBankAccount);
