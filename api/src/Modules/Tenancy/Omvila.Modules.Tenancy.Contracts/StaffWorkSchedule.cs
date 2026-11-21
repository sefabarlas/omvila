namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Personelin bu şubedeki çalışma düzeni.</summary>
/// <param name="StaffId">Personel.</param>
/// <param name="IsBookableOnline">"Online randevuya açık".</param>
/// <param name="Hours">Bu şubedeki haftalık saatleri.</param>
/// <param name="Overrides">Personele özel günler; modelde henüz yok (WorkingHoursOverride yalnız şube), boş döner.</param>
public sealed record StaffWorkSchedule(Guid StaffId, bool IsBookableOnline, IReadOnlyList<WeeklyHoursInterval> Hours, IReadOnlyList<HoursOverride> Overrides);
