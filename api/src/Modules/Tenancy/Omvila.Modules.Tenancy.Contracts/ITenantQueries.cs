namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Marka ve şube okuma (oturum özeti). Markanın kiracı bağlamında çağrılır.</summary>
public interface ITenantQueries
{
    /// <param name="brandId">Marka.</param>
    /// <param name="branchIds">Kapsam: null = bütün etkin şubeler (Owner), dolu = yalnız bu şubeler.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<TenantSummary?> GetSummaryAsync(Guid brandId, IReadOnlyCollection<Guid>? branchIds, CancellationToken ct);

    /// <summary>Verilen kimliklerden markanın etkin personeline ait olanlar.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="staffIds">Denetlenecek personel kimlikleri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlySet<Guid>> GetActiveStaffIdsAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct);

    /// <summary>Şube markaya ait mi (etkin olmasa da).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<bool> BranchExistsAsync(Guid brandId, Guid branchId, CancellationToken ct);
}
