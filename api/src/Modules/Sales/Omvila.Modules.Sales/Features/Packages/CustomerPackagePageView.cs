namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme CustomerPackagePage.</summary>
internal sealed record CustomerPackagePageView(IReadOnlyList<CustomerPackageView> Items, string? NextCursor);
