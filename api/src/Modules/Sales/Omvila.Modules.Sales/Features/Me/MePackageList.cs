namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Sözleşme MePackageList ("2 aktif paket"; önce kullanılabilirler, sonra bitenler).</summary>
internal sealed record MePackageList(int ActiveCount, IReadOnlyList<MePackage> Items);
