using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Randevu onaylı oluştu (A1: panel/telefon, onaysız online). Notifications <c>appointment.confirmed</c> ve
/// <c>appointment.reminder</c> outbox satırlarını bu olaydan üretir (bildirimler.md § Olay kataloğu). Müşteri SMS tercihi
/// (<c>AppointmentSmsEnabled</c>) ve salt okunur abonelik işleyicide uygulanır. Tekrarlayan randevuda her tekrar için
/// yayımlanır; onay mesajı yalnız <see cref="IsSeriesStart"/> için, hatırlatmalar her tekrar için planlanır.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="Channel">Online, Panel, Phone.</param>
/// <param name="NotifyCustomer">"Müşteriye SMS gönder" (online her zaman true).</param>
/// <param name="SeriesId">Tekrarlayan randevunun serisi.</param>
/// <param name="IsSeriesStart">Serinin oluşturma isteğindeki ilk randevusu (tekrarsız randevuda false).</param>
public sealed record AppointmentConfirmed(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    string Channel,
    bool NotifyCustomer,
    Guid? SeriesId,
    bool IsSeriesStart) : IIntegrationEvent;
