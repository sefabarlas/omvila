using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon randevuyu iptal etti (A7). Notifications <c>appointment.cancelled_by_salon</c> gönderir (anahtar kapalıysa
/// gönderilmez: "Müşteriye bildir") ve planlanmış hatırlatmaları düşürür. Kapora alınmışsa metne " Kapora iadesi için salon
/// sizinle iletişime geçecek." eklenir (bildirimler.md).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">İptal edilen randevunun başlangıcı.</param>
/// <param name="Reason">CancellationReason metni.</param>
/// <param name="NotifyCustomer">"Müşteriye bildir".</param>
/// <param name="DepositReceived">Kapora alınmış ve iade işaretlenmemiş.</param>
/// <param name="OfferToWaitlist">Boşalan saat bekleme listesindeki sıradaki uygun kişiye teklif edilsin (ManageCancelRequest.offerToWaitlist).</param>
public sealed record AppointmentCancelledBySalon(
    Guid BrandId,
    Guid AppointmentId,
    Guid BrandCustomerId,
    Instant StartUtc,
    string Reason,
    bool NotifyCustomer,
    bool DepositReceived = false,
    bool OfferToWaitlist = true) : IIntegrationEvent;
