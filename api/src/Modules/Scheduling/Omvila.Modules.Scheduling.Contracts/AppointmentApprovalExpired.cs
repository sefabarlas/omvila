using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Tutma süresi doldu, talep sistemce iptal edildi (A5; <c>ExpireAppointmentHolds</c> işi veya yazma yolunun satır içi
/// iptali). Notifications <see cref="Reason"/> <c>ApprovalExpired</c> ise müşteriye <c>appointment.approval_expired</c>
/// ("zamanında yanıtlayamadı"), <c>DepositNotReceived</c> ise <c>deposit.not_received</c> ("… kapora … gelmediği için …
/// iptal edildi") yazar. Scheduling boşalan saati bekleme listesine teklif eder (OfferVacatedSlotToWaitlist).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">İptal edilen talebin başlangıcı.</param>
/// <param name="Reason">CancellationReason metni: ApprovalExpired veya DepositNotReceived.</param>
/// <param name="DepositAmountMinor">İstenen kapora (kuruş); kapora istenmemişse null.</param>
public sealed record AppointmentApprovalExpired(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    string Reason,
    long? DepositAmountMinor = null) : IIntegrationEvent;
