using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Contracts;

/// <summary>
/// Personelin verdiği hizmetler ve personele özel fark ("Keratin 75 dk (özel)", "Balyaj +300 ₺"; alan-modeli.md
/// § ServiceStaff). Markanın kiracı bağlamında çağrılır.
/// </summary>
public interface IStaffServiceAssignments
{
    /// <param name="brandId">Marka.</param>
    /// <param name="staffId">Personel.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<StaffServiceAssignment>> GetAsync(Guid brandId, Guid staffId, CancellationToken ct);

    /// <summary>
    /// Verilen personelin etkin hizmet atamalarındaki personele özel prim oranları (yalnız oranı dolu olanlar; Sales hizmet primi).
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<StaffServiceCommission>> GetCommissionOverridesAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);

    /// <summary>
    /// Personelin hizmet listesini değiştirir. Hizmet markada aktif olmalı, özel süre aşamalara sığmalı, ek ücret
    /// hizmetin para biriminde olmalı; aksi alan hatası "{fieldPrefix}[i].…".
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffId">Personel.</param>
    /// <param name="assignments">Yeni liste.</param>
    /// <param name="fieldPrefix">Hata alan yolu öneki (ör. "services" veya "items").</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result> SetAsync(Guid brandId, Guid staffId, IReadOnlyList<StaffServiceAssignment> assignments, string fieldPrefix, CancellationToken ct);
}
