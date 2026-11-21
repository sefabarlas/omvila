using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Contracts;

/// <summary>
/// Randevu kalemlerinin kapora çözümü (sureler-ve-ayarlar.md § Kapora, "Bir kalem için kapora çözümü"): aktif kapora hesabı
/// yoksa, kanal Online değilse veya kalem paketten düşülüyorsa kapora yok; hizmetin modu (Never / Always / Inherit) ve
/// Inherit'te marka politikası (None / Always / RiskyCustomersOnly: müşterinin gelmeme sayısı ≥ marka eşiği). Yüzde kapora
/// liraya yuvarlanır. Marka politikası, eşik ve hesap Tenancy'den okunur. Markanın kiracı bağlamında çağrılır.
/// </summary>
public interface IServiceDeposits
{
    /// <summary>Kalemlerin kaporası, istekle aynı sırada; kapora yoksa o satır null. Randevu kaporası bunların toplamıdır.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="request">Kanal, müşterinin gelmeme sayısı ve kalemler.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<Money?>> ResolveAsync(Guid brandId, ServiceDepositRequest request, CancellationToken ct);
}
