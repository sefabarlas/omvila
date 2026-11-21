namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageItemWrite.</summary>
internal sealed record PackageItemWriteRequest(Guid? ServiceId, int? SessionCount);
