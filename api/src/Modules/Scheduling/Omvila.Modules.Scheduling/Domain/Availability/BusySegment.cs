using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Personelin aktif ve personel-meşgul bir dilimi, hangi şubede olursa olsun (musaitlik.md § 4). Başka şubedeyse geçiş
/// tamponu eklenir (§ 5).
/// </summary>
/// <param name="AppointmentId">Randevu; bekleme teklifi tutmasında null. Saat değişikliğinde kendi eski saati hariç tutulur.</param>
/// <param name="BranchId">Dilimin şubesi.</param>
/// <param name="During">[Start, End).</param>
/// <param name="HoldExpiresUtc">PendingApproval tutmasının bitişi; geçmişse saat boş sayılır (M17, yazma yolu iptal eder).</param>
/// <param name="ReservedForCustomerId">Bekleme teklifi tutması: yalnız teklif sahibine açık (M18).</param>
internal sealed record BusySegment(
    Guid? AppointmentId,
    Guid BranchId,
    Interval During,
    Instant? HoldExpiresUtc = null,
    Guid? ReservedForCustomerId = null);
