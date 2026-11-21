using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// KVKK silme (salon tarafı, yalnız o marka) genişleme noktası (guvenlik-kvkk.md § Anonimleştirme; alan-modeli.md §
/// DataDeletionRequest; T-92). Müşteri kartına bağlı verisi olan her modül önizleme sayılarını verir ve anonimleştirmede
/// kendi kayıtlarını anonim karta bağlı bırakır (işlem kayıtları silinmez). Customers modülü katılımcıları aynı istek
/// transaction'ında ve markanın kiracı bağlamında çağırır.
/// </summary>
public interface ICustomerErasureParticipant
{
    /// <summary>MusteriSil önizlemesine bu modülün katkısı ("Korunacak (anonim)", iptal edilecek randevular, aktif paket).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="brandCustomerId">Kart.</param>
    /// <param name="now">Şimdi (yaklaşan randevu sınırı).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<CustomerErasureFootprint> GetFootprintAsync(Guid brandId, Guid brandCustomerId, Instant now, CancellationToken ct);

    /// <summary>
    /// Anonimleştirme: yaklaşan kayıtları kapatır (randevu iptali, neden <c>AccountDeleted</c>, müşteriye bildirim yok) ve
    /// global kişi bağını anonim kişiye çevirir.
    /// </summary>
    /// <param name="erasure">Anonimleştirme.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task EraseAsync(CustomerErasure erasure, CancellationToken ct);
}
