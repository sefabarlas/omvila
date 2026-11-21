using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Randevunun saati veya personeli değişti (A14). Notifications <c>appointment.rescheduled</c> gönderir ve hatırlatmaları
/// yeniden planlar (bildirimler.md).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="PreviousStartUtc">Eski başlangıç.</param>
/// <param name="StartUtc">Yeni başlangıç.</param>
/// <param name="ChangedBy">Customer veya Salon.</param>
/// <param name="NotifyCustomer">Müşteriye mesaj gitsin mi (salonun anahtarı; müşteri değiştirdiyse salona bildirim).</param>
public sealed record AppointmentRescheduled(
    Guid BrandId,
    Guid AppointmentId,
    Guid BrandCustomerId,
    Instant PreviousStartUtc,
    Instant StartUtc,
    string ChangedBy,
    bool NotifyCustomer) : IIntegrationEvent;
