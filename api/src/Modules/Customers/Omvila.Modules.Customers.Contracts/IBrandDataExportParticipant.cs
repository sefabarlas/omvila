namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// "Tüm verileri dışa aktar" (sözleşme manageRequestDataExport; guvenlik-kvkk.md § Haklar: erişim) genişleme noktası. Markaya ait
/// işlem verisi olan her modül kendi tablolarını verir; Customers modülü dosyaları CSV'ye çevirip tek arşivde toplar.
/// Arka plan işinde, markanın kiracı bağlamında çağrılır.
/// </summary>
public interface IBrandDataExportParticipant
{
    /// <summary>Markanın bu modüldeki verisi (bir veya birkaç tablo).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<BrandDataExportFile>> ExportAsync(Guid brandId, CancellationToken ct);
}
