using Omvila.Modules.Sales.Contracts;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>publicListMyPackages yanıtı (items: UsablePackageItem[]).</summary>
internal sealed record PackageList(IReadOnlyList<UsablePackageItem> Items);
