namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageRefundRequest (tutarı salon elle girer; S-10, W9).</summary>
internal sealed record PackageRefundRequest(MoneyInput? Amount, string? Note);
