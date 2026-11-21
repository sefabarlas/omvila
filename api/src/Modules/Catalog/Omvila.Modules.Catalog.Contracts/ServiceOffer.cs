using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Kaleme kopyalanacak çözülmüş hizmet teklifi.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="StaffId">Çözümde kullanılan personel.</param>
/// <param name="ServiceName">Hizmet adı (kalemde ServiceNameSnapshot).</param>
/// <param name="Price">Kalem fiyatı: şube fiyatı ?? hizmet fiyatı, personel ek ücreti dahil.</param>
/// <param name="StaffSurcharge">Fiyatın içindeki personel ek ücreti (yoksa sıfır).</param>
/// <param name="IsBranchPrice">Fiyat şube fiyatından mı geldi.</param>
/// <param name="PriceDisplay">Fiyat gösterimi.</param>
/// <param name="DurationMinutes">Çözülmüş süre (aşamaların toplamı).</param>
/// <param name="BufferBeforeMinutes">Önce tampon; personel meşgul sayılır.</param>
/// <param name="BufferAfterMinutes">Sonra tampon; personel meşgul sayılır.</param>
/// <param name="Stages">Aşamalar sırasıyla; aşamasız hizmette tek örtük aşama (personel meşgul, süre = <paramref name="DurationMinutes"/>).</param>
/// <param name="RequiresApproval">Hizmetin onay ayarı; <c>null</c> = marka ayarı.</param>
/// <param name="IsOnlineBookable">Online randevuya açık mı.</param>
/// <param name="MinNoticeMinutes">Hizmetin "En az ön bildirim" ezmesi; <c>null</c> = marka ayarı.</param>
/// <param name="MaxAdvanceDays">Hizmetin "En ileri tarih" ezmesi; <c>null</c> = marka ayarı.</param>
public sealed record ServiceOffer(
    Guid ServiceId,
    Guid? StaffId,
    string ServiceName,
    Money Price,
    Money StaffSurcharge,
    bool IsBranchPrice,
    ServicePriceDisplay PriceDisplay,
    int DurationMinutes,
    int BufferBeforeMinutes,
    int BufferAfterMinutes,
    IReadOnlyList<ServiceOfferStage> Stages,
    bool? RequiresApproval,
    bool IsOnlineBookable,
    int? MinNoticeMinutes,
    int? MaxAdvanceDays);
