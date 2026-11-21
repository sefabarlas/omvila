namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Bir başlangıcın neden uygun olmadığı (musaitlik.md § Panelden elle ekleme). <see cref="OutsideWorkingHours"/> ve
/// <see cref="StaffOnTimeOff"/> panelde uyarıdır (ikinci onayla geçilir); diğerleri hiçbir kipte geçilemez.
/// </summary>
[Flags]
internal enum SlotViolation
{
    None = 0,

    /// <summary>Şube kapalı veya personelin çalışma saati dışı (409 outside_working_hours).</summary>
    OutsideWorkingHours = 1 << 0,

    /// <summary>Personel izinli (409 staff_on_time_off).</summary>
    StaffOnTimeOff = 1 << 1,

    /// <summary>Personel başka randevuda veya şubeler arası geçişte (slot_taken).</summary>
    StaffBusy = 1 << 2,

    /// <summary>Gereken kaynak dolu veya bakımda.</summary>
    ResourceUnavailable = 1 << 3,

    /// <summary>Şimdi + MinNoticeMinutes öncesi (yalnız public).</summary>
    BeforeMinNotice = 1 << 4,

    /// <summary>Bugün + MaxAdvanceDays sonrası (yalnız public).</summary>
    BeyondMaxAdvance = 1 << 5,

    /// <summary>Başlangıç ızgaraya hizalı değil (yalnız public).</summary>
    OffGrid = 1 << 6,

    /// <summary>Personel veya hizmet online randevuya kapalı (yalnız public).</summary>
    NotBookableOnline = 1 << 7,

    /// <summary>Hizmeti bu şubede veren personel yok veya istenen personel hizmeti vermiyor.</summary>
    NoEligibleStaff = 1 << 8,

    /// <summary>Fark etmez: bu başlangıçta bütün kalemleri yapabilecek boş personel yok.</summary>
    NoStaffAvailable = 1 << 9,
}
