namespace Omvila.Modules.Platform.Contracts;

/// <summary>Abonelik okuma (panel şeritleri). Markanın kiracı bağlamında çağrılır.</summary>
public interface ISubscriptionQueries
{
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<SubscriptionBannerInfo?> GetBannerAsync(Guid brandId, CancellationToken ct);

    /// <summary>Aboneliğin planındaki sınırlar; abonelik yoksa sınırsız değil, null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PlanLimitInfo?> GetPlanLimitsAsync(Guid brandId, CancellationToken ct);
}
