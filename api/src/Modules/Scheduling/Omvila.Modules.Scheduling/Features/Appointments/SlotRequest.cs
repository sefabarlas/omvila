using NodaTime;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// İstenen saat (sözleşme ManageAppointmentCreate / PublicAppointmentCreate ortak kısmı). Panel kipinde pencere ve ızgara
/// uygulanmaz; <see cref="AllowOutsideWorkingHours"/> yalnız panelde dikkate alınır (çalışma saati ve izin uyarısının
/// ikinci onayı); çakışma hiçbir kipte geçilemez (musaitlik.md § Panelden elle ekleme).
/// </summary>
/// <param name="BranchId">Randevunun şubesi (markada olmalı; değilse not_found).</param>
/// <param name="StartUtc">İlk kalemin başlangıcı; diğer kalemler zincirlenir.</param>
/// <param name="Items">Kalemler; personel null = fark etmez.</param>
/// <param name="SameStaffForAll">"İki hizmet de aynı kişiden".</param>
/// <param name="Mode">Public (rezervasyon sayfası) veya Panel.</param>
/// <param name="AllowOutsideWorkingHours">Panelde çalışma saati / izin uyarısı onaylandı.</param>
/// <param name="PreferredStaffId">Müşterinin tercih ettiği personel (fark etmez eşitliğinde).</param>
internal sealed record SlotRequest(
    Guid BranchId,
    Instant StartUtc,
    IReadOnlyList<RequestedItem> Items,
    bool SameStaffForAll,
    AvailabilityMode Mode,
    bool AllowOutsideWorkingHours = false,
    Guid? PreferredStaffId = null)
{
    /// <summary>Uyarı sınıfı ihlaller yalnız panelde ve açık onayla geçilir.</summary>
    public bool OverridesWarnings => Mode == AvailabilityMode.Panel && AllowOutsideWorkingHours;
}
