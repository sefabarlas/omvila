using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon onay bekleyen talebi reddetti (A4 → Cancelled, neden DeclinedBySalon). Notifications müşteriye her zaman
/// <c>appointment.declined</c> yazar ("talebinizi onaylayamadı, talep iptal edildi"; "reddedildi" geçmez) ve bekleyen
/// satırları düşürür. Bekleme listesi Dilim 5'te.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">İptal edilen talebin başlangıcı.</param>
/// <param name="DeclinedByUserId">Reddeden kullanıcı.</param>
public sealed record AppointmentDeclined(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    Guid? DeclinedByUserId) : IIntegrationEvent;
