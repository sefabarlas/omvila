namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Takvim kolonu ve randevu ekranlarında personel (sözleşme CalendarView.staff, AppointmentItemView.staffName).</summary>
/// <param name="StaffId">Personel.</param>
/// <param name="FullName">Ad soyad.</param>
/// <param name="DisplayName">Görünen ad (varsa).</param>
/// <param name="CalendarColor">Takvim rengi.</param>
/// <param name="PhotoUrl">Fotoğraf (henüz yok, null).</param>
public sealed record StaffCard(Guid StaffId, string FullName, string? DisplayName, string CalendarColor, string? PhotoUrl);
