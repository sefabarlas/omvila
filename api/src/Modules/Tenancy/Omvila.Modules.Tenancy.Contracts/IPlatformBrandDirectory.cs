using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Platform paneli için marka okuma ve marka durumu yazımı (Markalar, marka detayı, genel bakış; K4). Platform okuma servisidir
/// (ADR-0006): yalnız admin yüzeyinin platform bağlamında çağrılır, başka bağlamda istisna atar. Platform marka verisini
/// değiştirmez; tek yazım platform kavramı olan durumdur (askıya alma / yeniden açma).
/// </summary>
public interface IPlatformBrandDirectory
{
    /// <summary>Arşivlenmemiş bütün markalar (platform ölçeğinde marka sayısı küçüktür; birleştirme Platform'da).</summary>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<PlatformBrandRow>> ListAsync(CancellationToken ct);

    /// <summary>Tek marka ve şubeleri; yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PlatformBrandProfile?> GetAsync(Guid brandId, CancellationToken ct);

    /// <summary>Active ↔ Suspended; marka yoksa 404, geçiş geçersizse 409 invalid_state_transition.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="status"><c>Active</c> veya <c>Suspended</c>.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result> ChangeStatusAsync(Guid brandId, string status, CancellationToken ct);
}
