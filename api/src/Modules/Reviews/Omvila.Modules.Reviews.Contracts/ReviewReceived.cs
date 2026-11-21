using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Contracts;

/// <summary>
/// Müşteri değerlendirme gönderdi (sözleşme publicCreateReview). Notifications <c>staff.review_received</c> push'u ve panel kaydı
/// yazar; alıcılar review.view izinli kullanıcılardır (Owner ve şubenin yöneticileri; personel almaz, W8).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="ReviewId">Değerlendirme.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube (alıcı kapsamı).</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="Rating">Puan (1–5).</param>
public sealed record ReviewReceived(
    Guid BrandId,
    Guid ReviewId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    int Rating) : IIntegrationEvent;
