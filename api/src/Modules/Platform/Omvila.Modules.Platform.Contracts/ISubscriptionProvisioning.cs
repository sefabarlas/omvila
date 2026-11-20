namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Kayıtta deneme aboneliği (dilim-01 § Alan kuralları): Subscription(Trialing, TrialEndsOn = bugün + TrialDays) ve
/// yapılandırmadaki deneme kontörü. İstek transaction'ı içinde, yeni markanın kiracı bağlamında çağrılır.
/// </summary>
public interface ISubscriptionProvisioning
{
    /// <param name="brandId">Marka.</param>
    /// <param name="timeZoneId">"Bugün"ün hesaplandığı saat dilimi (ilk şubenin).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<StartedTrial> StartTrialAsync(Guid brandId, string timeZoneId, CancellationToken ct);
}
