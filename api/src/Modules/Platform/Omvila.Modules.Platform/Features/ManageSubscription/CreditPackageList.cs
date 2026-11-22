using System.Text.Json.Serialization;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme CreditPackageList.</summary>
internal sealed record CreditPackageList(IReadOnlyList<CreditPackageView> Items);
