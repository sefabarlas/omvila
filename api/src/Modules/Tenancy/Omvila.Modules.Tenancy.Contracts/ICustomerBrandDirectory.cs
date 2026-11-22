namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Müşteri uygulamasının (me yüzeyi) marka okumaları: müşteri bağlamında marka satırları görünmez, okuma platform bağlamında
/// yapılır ve oturumun önceki bağlamı geri yüklenir. Arşivlenmiş marka yer almaz.
/// </summary>
public interface ICustomerBrandDirectory
{
    /// <summary>Bağlantı adını (güncel veya yönlendirmesi süren eski ad) markaya çözer; bulunamazsa null.</summary>
    /// <param name="slug">Bağlantı adı.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Guid?> ResolveSlugAsync(string slug, CancellationToken ct);

    /// <summary>Verilen markaların kartları (sözleşme MeBrand, MeBrandRef); bulunmayan veya arşivlenmiş marka yer almaz.</summary>
    /// <param name="brandIds">Markalar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, CustomerBrandCard>> GetCardsAsync(IReadOnlyCollection<Guid> brandIds, CancellationToken ct);
}
