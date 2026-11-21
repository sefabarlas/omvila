using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon onay bekleyen talebi onayladı (A2 → Confirmed). Notifications müşteriye <c>appointment.approved</c> ("talebinizi
/// onayladı") yazar ve hatırlatmaları planlar. Onaylanan talep için <see cref="AppointmentConfirmed"/> ayrıca yayımlanmaz.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="ApprovedByUserId">Onaylayan kullanıcı.</param>
public sealed record AppointmentApproved(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    Guid? ApprovedByUserId) : IIntegrationEvent;
