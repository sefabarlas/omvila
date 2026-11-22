using System.Text.Json.Serialization;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme CreditPurchaseRequest.</summary>
internal sealed record CreditPurchaseRequest(Guid? CreditPackageId);
