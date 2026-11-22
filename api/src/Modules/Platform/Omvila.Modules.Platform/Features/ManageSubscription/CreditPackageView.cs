using System.Text.Json.Serialization;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme CreditPackage.</summary>
internal sealed record CreditPackageView(
    Guid Id, string Code, string Name, int Credits, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? Price)
{
    public static CreditPackageView From(CreditPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);
        return new CreditPackageView(
            package.Id, package.Code, package.Name, package.Credits, package.PriceMinor is { } price ? Money.Try(price) : null);
    }
}
