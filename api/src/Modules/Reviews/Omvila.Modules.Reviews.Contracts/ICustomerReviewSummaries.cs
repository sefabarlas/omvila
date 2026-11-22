namespace Omvila.Modules.Reviews.Contracts;

/// <summary>
/// Müşteri uygulamasının randevu satırlarındaki değerlendirme özeti (sözleşme MeAppointmentReview; "Değerlendirilmedi" süzgeci).
/// Randevular birden çok markada olduğundan okuma platform bağlamında, yalnız verilen randevu kimlikleriyle süzülerek yapılır;
/// oturumun önceki bağlamı geri yüklenir.
/// </summary>
public interface ICustomerReviewSummaries
{
    /// <summary>Değerlendirilmiş randevuların özeti; değerlendirilmemiş randevu sonuçta yer almaz.</summary>
    /// <param name="appointmentIds">Randevular (çağıran, müşterinin kendi randevularını verir).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, AppointmentReviewSummary>> GetByAppointmentsAsync(IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct);
}
