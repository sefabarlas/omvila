using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Tutma süresi doldu, talep sistemce iptal edildi (A5; <c>ExpireAppointmentHolds</c> işi veya yazma yolunun satır içi
/// iptali). Notifications <see cref="Reason"/> <c>ApprovalExpired</c> ise müşteriye <c>appointment.approval_expired</c>
/// ("zamanında yanıtlayamadı") yazar; <c>DepositNotReceived</c> Dilim 4'te kapora metniyle gelir. Bekleme listesi Dilim 5'te.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">İptal edilen talebin başlangıcı.</param>
/// <param name="Reason">CancellationReason metni: ApprovalExpired veya DepositNotReceived.</param>
public sealed record AppointmentApprovalExpired(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    string Reason) : IIntegrationEvent;
