using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Contracts;

/// <summary>
/// Randevu kalemine kopyalanacak fiyat ve sürenin çözümü (alan-modeli.md § BranchServicePrice "Fiyat ve süre çözümü"):
/// fiyat = şube fiyatı ?? hizmet fiyatı, + personel ek ücreti; süre = personel özel süresi ?? şube süresi ?? hizmet
/// süresi, fark son personel-meşgul aşamaya uygulanır. Markanın kiracı bağlamında çağrılır.
/// </summary>
public interface IServiceOffers
{
    /// <summary>
    /// İstekleri sırasıyla çözer; sonuç listesi istek listesiyle aynı sıradadır. Hizmet markada yok, pasif veya şubede
    /// verilmiyorsa ya da personel hizmeti vermiyorsa o satır <c>not_found</c> hatasıdır. Sorgu sayısı istek sayısından
    /// bağımsızdır.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube (şube fiyatı ve şubede verilip verilmediği).</param>
    /// <param name="requests">Hizmet ve personel çiftleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<Result<ServiceOffer>>> ResolveAsync(
        Guid brandId, Guid branchId, IReadOnlyList<ServiceOfferRequest> requests, CancellationToken ct);

    /// <summary>Tek hizmet ve personel için <see cref="ResolveAsync(Guid, Guid, IReadOnlyList{ServiceOfferRequest}, CancellationToken)"/>.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="serviceId">Hizmet.</param>
    /// <param name="staffId">Personel; <c>null</c> personelsiz çözüm (ek ücret ve özel süre yok).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result<ServiceOffer>> ResolveAsync(Guid brandId, Guid branchId, Guid serviceId, Guid? staffId, CancellationToken ct);
}
