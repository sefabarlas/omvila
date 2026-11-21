using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon "Tamamla ve ödeme al" dedi (A11 → Completed). Tahsilat, hizmet primi ve müşteri sayaçları aynı transaction'da yazılmıştır;
/// olay sonraki yan etkiler içindir (değerlendirme daveti <c>review.invite</c>: ReviewInviteEnabled açıksa ReviewInviteDelayMinutes
/// sonra; docs/acik-sorular.md T-88).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="CompletedUtc">Tamamlama anı.</param>
/// <param name="StaffIds">Kalemlerin personeli (tekrarsız, kalem sırasıyla; personelsiz kalem yer almaz).</param>
/// <param name="CompletedByUserId">Tamamlayan kullanıcı.</param>
public sealed record AppointmentCompleted(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant CompletedUtc,
    IReadOnlyList<Guid> StaffIds,
    Guid? CompletedByUserId) : IIntegrationEvent;
