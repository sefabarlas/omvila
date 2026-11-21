namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>publicListMyPackages yanıtı (items: UsablePackageItem[]; paketler Dilim 5).</summary>
internal sealed record PackageList(IReadOnlyList<object> Items);
