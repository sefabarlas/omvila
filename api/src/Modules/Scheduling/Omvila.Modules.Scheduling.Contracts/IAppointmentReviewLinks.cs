namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Değerlendirme bağlantısı (<c>/d/{code}</c>; alan-modeli.md § AppointmentAccessCode, Purpose <c>Review</c>) ve değerlendirilecek
/// randevu bilgisi (Reviews modülü). Kod tablosu Scheduling'dedir; çağıranın transaction'ında çalışır.
/// </summary>
public interface IAppointmentReviewLinks
{
    /// <summary>Randevunun değerlendirme bilgisi; randevu markada yoksa null. Kiracı bağlamı kurulmuş olmalıdır.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentId">Randevu.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<AppointmentReviewSubject?> GetSubjectAsync(Guid brandId, Guid appointmentId, CancellationToken ct);

    /// <summary>
    /// Randevunun geçerli <c>/d/</c> kodunu döner, yoksa üretir (tekrar çağrı aynı kodu verir). Geçerlilik randevu bitişinden (bitiş
    /// geçmişse şimdiden) <c>AccessCodeValidityAfterEndDays</c> gün. Randevu markada yoksa null. Kiracı bağlamı kurulmuş olmalıdır.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentId">Randevu.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<string?> IssueReviewCodeAsync(Guid brandId, Guid appointmentId, CancellationToken ct);

    /// <summary>
    /// <c>/d/</c> kodunu platform bağlamında çözer; geçerliyse oturumun kiracı bağlamı randevunun markasına kurulur ve randevu
    /// döner. Biçimi bozuk, bilinmeyen, süresi dolmuş veya iptal edilmiş kod ayrımsız null (bağlam değişmez).
    /// </summary>
    /// <param name="code">Bağlantıdaki kod.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<AppointmentReviewSubject?> EnterByReviewCodeAsync(string? code, CancellationToken ct);

    /// <summary>
    /// Müşteri uygulamasından değerlendirme (sözleşme meCreateReview): randevu platform bağlamında müşterinin global kimliğiyle aranır;
    /// müşterinin randevusuysa oturumun kiracı bağlamı randevunun markasına kurulur ve randevu döner. Başka müşterinin randevusu veya
    /// taslak null (bağlam değişmez).
    /// </summary>
    /// <param name="customerId">Global müşteri (me jetonunun sub'ı).</param>
    /// <param name="appointmentId">Randevu.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<AppointmentReviewSubject?> EnterForCustomerAsync(Guid customerId, Guid appointmentId, CancellationToken ct);
}
