namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Hizmet adları ve etkinliği (Sales: paket içeriği ve müşteri paketi görünümü). Markanın kiracı bağlamında çağrılır.</summary>
public interface IServiceDirectory
{
    /// <summary>Markadaki hizmetler (pasif olsalar da; geçmiş kayıtta ad görünür). Bulunmayan kimlikler sonuçta yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="serviceIds">Hizmet kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, ServiceSummary>> GetSummariesAsync(Guid brandId, IReadOnlyCollection<Guid> serviceIds, CancellationToken ct);
}
