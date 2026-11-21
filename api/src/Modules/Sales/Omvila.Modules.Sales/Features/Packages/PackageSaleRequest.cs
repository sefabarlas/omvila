namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageSaleRequest ("Müşteriye paket sat"; indirim yok, tutar paket fiyatı).</summary>
internal sealed record PackageSaleRequest(Guid? PackageId, Guid? BranchId, Guid? SoldByStaffId, string? PaymentMethod);
