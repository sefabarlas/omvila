using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Değerlendirilecek randevu (Reviews: davet, /d/ bağlantısı ve değerlendirme kaydı). Not ve kişisel metin taşımaz.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="Status">AppointmentStatus metni (güncel durum; yalnız "Completed" değerlendirilebilir).</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="ServiceNames">Kalemlerin kopyalanan hizmet adları, sırayla.</param>
/// <param name="StaffId">
/// Değerlendirmenin personeli: kalem sırasına göre personeli olan ilk kalemin personeli (çok kalemli randevuda; docs/acik-sorular.md
/// T-89); personelsiz randevuda null.
/// </param>
public sealed record AppointmentReviewSubject(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    string Status,
    Instant StartUtc,
    IReadOnlyList<string> ServiceNames,
    Guid? StaffId)
{
    /// <summary>Yalnız tamamlanan randevu değerlendirilir (alan-modeli.md § Review).</summary>
    public bool IsCompleted => string.Equals(Status, "Completed", StringComparison.Ordinal);
}
