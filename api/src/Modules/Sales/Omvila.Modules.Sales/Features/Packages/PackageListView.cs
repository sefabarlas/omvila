namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageList.</summary>
internal sealed record PackageListView(IReadOnlyList<PackageDefinitionView> Items, PackageOverviewView Overview);
