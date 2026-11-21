using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>Paket tanımının doğrulanmış içeriği ("Paket oluştur"; sözleşme PackageWrite).</summary>
/// <param name="Name">Ad (2–80).</param>
/// <param name="Price">Fiyat (sıfırdan büyük).</param>
/// <param name="Items">İçerik: hizmet ve seans sayısı (1–10 satır, hizmet tekrar etmez).</param>
/// <param name="ValidityDays">Geçerlilik, satıştan itibaren gün.</param>
/// <param name="SaleCommissionPercent">Pakete özel satış primi; null ise satan personelin oranı.</param>
/// <param name="IsAvailableAllBranches">"Bütün şubelerde kullanılabilir".</param>
/// <param name="IsActive">"Satışta".</param>
internal sealed record PackageDraft(
    string Name,
    Money Price,
    IReadOnlyList<PackageDraftItem> Items,
    int ValidityDays,
    decimal? SaleCommissionPercent,
    bool IsAvailableAllBranches,
    bool IsActive);
