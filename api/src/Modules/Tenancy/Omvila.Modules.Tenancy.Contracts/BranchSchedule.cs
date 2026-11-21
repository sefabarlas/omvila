namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Şubenin müsaitlik girdileri.</summary>
/// <param name="BranchId">Şube.</param>
/// <param name="TimeZoneId">IANA saat dilimi.</param>
/// <param name="IsActive">Şube etkin mi.</param>
/// <param name="Rules">Randevu kuralları (marka; ızgara şube ezmesiyle).</param>
/// <param name="Hours">Haftalık saatler; günde birden çok aralık olabilir.</param>
/// <param name="Overrides">İstenen aralıktaki özel günler.</param>
/// <param name="Staff">Şubede etkin çalışan (istenen) personel.</param>
/// <param name="TimeOffs">Bu personelin aralığa değen bütün izinleri (şubeden bağımsız).</param>
/// <param name="Transfers">Çifte özel geçiş süreleri; BranchTransferTime tablosu henüz yok, boş döner.</param>
public sealed record BranchSchedule(
    Guid BranchId,
    string TimeZoneId,
    bool IsActive,
    BookingRules Rules,
    IReadOnlyList<WeeklyHoursInterval> Hours,
    IReadOnlyList<HoursOverride> Overrides,
    IReadOnlyList<StaffWorkSchedule> Staff,
    IReadOnlyList<StaffTimeOff> TimeOffs,
    IReadOnlyList<BranchTransfer> Transfers);
