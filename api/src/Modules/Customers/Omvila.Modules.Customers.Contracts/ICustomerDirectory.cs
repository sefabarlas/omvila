using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// Müşteri dizini: diğer modüllerin (Scheduling, Sales) müşteriyle ilgili senkron ihtiyaçları (mimari.md § Modüller arası
/// iletişim: "Randevu oluşurken ICustomerDirectory.EnsureBrandCustomer"). Çağıranın transaction'ında ve markanın kiracı
/// bağlamında çalışır. Başka markanın kartı bulunamaz (null / not_found).
/// </summary>
public interface ICustomerDirectory
{
    /// <summary>
    /// Telefonla global kişiyi bulur veya oluşturur, markada kartı yoksa açar. Mevcut kişinin adı veya başka markadaki
    /// verisi değişmez ve döndürülmez. Eşzamanlı çağrılar aynı kişide buluşur (telefon benzersiz).
    /// </summary>
    /// <param name="request">İstek.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<EnsuredBrandCustomer> EnsureBrandCustomerAsync(EnsureBrandCustomerRequest request, CancellationToken ct);

    /// <summary>Markadaki kartların görünen bilgisi; bulunmayan kimlikler sonuçta yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="brandCustomerIds">Kart kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, CustomerDisplayInfo>> GetDisplayInfoAsync(Guid brandId, IReadOnlyCollection<Guid> brandCustomerIds, CancellationToken ct);

    /// <summary>Sayaçları günceller (tamamlanan ziyaret, iptal, gelmeme). Kart yoksa not_found.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="brandCustomerId">Kart.</param>
    /// <param name="change">Fark.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result> ApplyCountersAsync(Guid brandId, Guid brandCustomerId, CustomerCounterChange change, CancellationToken ct);

    /// <summary>Rıza satırı ekler (online randevuda aydınlatma metni ve kampanya izni).</summary>
    /// <param name="entry">Rıza.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task RecordConsentAsync(ConsentEntry entry, CancellationToken ct);
}
