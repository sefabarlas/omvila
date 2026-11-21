using NodaTime;

namespace Omvila.Modules.Reviews.Contracts;

/// <summary>
/// Değerlendirme daveti planlaması (durum-makinesi.md A11 "değerlendirme daveti planlanır"; bildirimler.md review.invite).
/// <c>AppointmentCompleted</c> olayının işleyicisi çağırır. Çağıranın transaction'ında çalışır.
/// </summary>
public interface IReviewInvitations
{
    /// <summary>
    /// Marka "Değerlendirme daveti" açıksa (<c>ReviewInviteEnabled</c>) ve randevu tamamlanmış, değerlendirilmemiş ve daha önce davet
    /// edilmemişse: /d/ kodunu üretir, daveti kaydeder ve <see cref="ReviewInviteScheduled"/> yayımlar (gönderim
    /// <paramref name="completedUtc"/> + <c>ReviewInviteDelayMinutes</c>). Diğer durumlarda hiçbir şey yazmaz (tekrar çağrı güvenli).
    /// Kiracı bağlamını markaya kurar.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentId">Tamamlanan randevu.</param>
    /// <param name="completedUtc">Tamamlanma anı.</param>
    /// <param name="ct">İptal belirteci.</param>
    /// <returns>Davet planlandıysa true.</returns>
    Task<bool> ScheduleAsync(Guid brandId, Guid appointmentId, Instant completedUtc, CancellationToken ct);
}
