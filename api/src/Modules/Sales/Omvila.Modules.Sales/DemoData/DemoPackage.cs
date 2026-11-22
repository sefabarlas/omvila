using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.DemoData;

/// <summary>Demo paket tanımı ("Paket oluştur"): tek hizmetli, bütün şubelerde geçerli, satışta.</summary>
/// <param name="Name">Ad.</param>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="SessionCount">Seans sayısı.</param>
/// <param name="Price">Fiyat.</param>
/// <param name="ValidityDays">Geçerlilik, satıştan itibaren gün.</param>
/// <param name="SaleCommissionPercent">Pakete özel satış primi; null ise satan personelin oranı.</param>
public sealed record DemoPackage(string Name, Guid ServiceId, int SessionCount, Money Price, int ValidityDays, decimal? SaleCommissionPercent);
