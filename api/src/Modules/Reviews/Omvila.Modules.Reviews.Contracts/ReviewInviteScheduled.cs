using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Contracts;

/// <summary>
/// Değerlendirme daveti planlandı (bildirimler.md review.invite). Notifications müşteriye <paramref name="SendAtUtc"/> zamanlı
/// "Nasıldı?" push'unu (cihaz yoksa SMS) yazar; randevu iptal olursa bekleyen satır düşer (bildirimler.md § İptal).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="AccessCode">Değerlendirme bağlantısının (/d/) kodu.</param>
/// <param name="SendAtUtc">Gönderim zamanı (tamamlanma + ReviewInviteDelayMinutes).</param>
public sealed record ReviewInviteScheduled(Guid BrandId, Guid AppointmentId, string AccessCode, Instant SendAtUtc) : IIntegrationEvent;
