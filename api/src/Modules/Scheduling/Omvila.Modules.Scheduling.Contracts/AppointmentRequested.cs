using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Onay gerektiren online talep oluştu (A1 → PendingApproval) veya müşteri saat değiştirince talep yeniden onaya düştü
/// (A13, <see cref="IsReschedule"/>). Notifications müşteriye <c>appointment.requested</c> ("Talebiniz alındı"), Owner ve
/// BranchManager kullanıcılarına <c>staff.approval_needed</c> yazar (bildirimler.md). Onaylı randevu için yayımlanmaz
/// (<see cref="AppointmentConfirmed"/>); hatırlatmalar talep onaylanınca (<see cref="AppointmentApproved"/>) planlanır.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="HoldExpiresUtc">Bu anda yanıtlanmamışsa otomatik iptal (A5); "1 sa 35 dk kaldı".</param>
/// <param name="IsReschedule">A13: onaylı randevunun müşteri tarafından saati değişti, yeniden onay bekliyor.</param>
public sealed record AppointmentRequested(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    Instant HoldExpiresUtc,
    bool IsReschedule) : IIntegrationEvent;
