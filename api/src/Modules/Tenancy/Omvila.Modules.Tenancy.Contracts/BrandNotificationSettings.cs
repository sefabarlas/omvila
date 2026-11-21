namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Bildirim modülünün marka ayarları (sureler-ve-ayarlar.md § Bildirim ve iletişim; sözleşme NotificationSettings).</summary>
/// <param name="ReminderOffsetsMinutes">"Randevu hatırlatma zamanları" (dakika, randevudan önce); boş = hatırlatma yok.</param>
/// <param name="ReviewInviteEnabled">"Değerlendirme daveti".</param>
/// <param name="TimeZoneId">Markanın ana (ilk etkin) şubesinin saat dilimi: marka düzeyindeki olaylar ve "bu ay" bu dilimde.</param>
public sealed record BrandNotificationSettings(IReadOnlyList<int> ReminderOffsetsMinutes, bool ReviewInviteEnabled, string TimeZoneId);
