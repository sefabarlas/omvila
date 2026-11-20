namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Marka ve şube okuma (oturum özeti). Markanın kiracı bağlamında çağrılır.</summary>
public interface ITenantQueries
{
    /// <param name="brandId">Marka.</param>
    /// <param name="branchIds">Kapsam: null = bütün etkin şubeler (Owner), dolu = yalnız bu şubeler.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<TenantSummary?> GetSummaryAsync(Guid brandId, IReadOnlyCollection<Guid>? branchIds, CancellationToken ct);
}
