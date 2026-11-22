using NodaTime;
using Omvila.Modules.Sales.Contracts;

namespace Omvila.Modules.Sales.DemoData;

/// <summary>Demo paket satışı ("Paket sat"); satış anı geçmişte olabilir (son kullanım günü satış gününden sayılır).</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="PackageId">Tanım.</param>
/// <param name="BranchId">Satış şubesi.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="SoldByStaffId">Satan personel (satış primi).</param>
/// <param name="Method">Tahsilat yöntemi.</param>
/// <param name="SoldUtc">Satış anı.</param>
/// <param name="SaleDate">Satışın şube yerel günü.</param>
public sealed record DemoPackageSale(
    Guid BrandId, Guid PackageId, Guid BranchId, Guid BrandCustomerId, Guid SoldByStaffId, CollectedPaymentMethod Method, Instant SoldUtc, LocalDate SaleDate);
