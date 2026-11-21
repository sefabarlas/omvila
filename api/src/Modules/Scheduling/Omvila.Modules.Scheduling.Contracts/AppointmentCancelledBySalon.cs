using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon randevuyu iptal etti (A7). Notifications <c>appointment.cancelled_by_salon</c> gönderir (anahtar kapalıysa
/// gönderilmez: "Müşteriye bildir") ve planlanmış hatırlatmaları düşürür.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">İptal edilen randevunun başlangıcı.</param>
/// <param name="Reason">CancellationReason metni.</param>
/// <param name="NotifyCustomer">"Müşteriye bildir".</param>
public sealed record AppointmentCancelledBySalon(
    Guid BrandId,
    Guid AppointmentId,
    Guid BrandCustomerId,
    Instant StartUtc,
    string Reason,
    bool NotifyCustomer) : IIntegrationEvent;
