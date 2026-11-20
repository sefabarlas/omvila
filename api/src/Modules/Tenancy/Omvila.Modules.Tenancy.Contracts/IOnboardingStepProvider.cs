namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Bir kurulum adımının tamamlanma kuralı. Adımın verisi hangi modüldeyse o modül kaydeder (ör. Services → Catalog).
/// Kaydı olmayan adım tamamlanmamış sayılır. Markanın kiracı bağlamında çağrılır.
/// </summary>
public interface IOnboardingStepProvider
{
    /// <summary>Değerlendirilen adım.</summary>
    OnboardingStep Step { get; }

    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct);
}
