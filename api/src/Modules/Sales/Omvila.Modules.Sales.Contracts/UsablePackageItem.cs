using NodaTime;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>Kullanılabilir paket kalemi (sözleşme UsablePackageItem).</summary>
/// <param name="CustomerPackageItemId">Müşterinin paket kalemi.</param>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="PackageName">Paket adı (satıştaki kopya).</param>
/// <param name="RemainingSessions">Kalan seans (defterden).</param>
/// <param name="TotalSessions">Kalemin toplam seansı.</param>
/// <param name="ExpiresOn">Son kullanım günü.</param>
public sealed record UsablePackageItem(
    Guid CustomerPackageItemId,
    Guid ServiceId,
    string PackageName,
    int RemainingSessions,
    int TotalSessions,
    LocalDate? ExpiresOn);
