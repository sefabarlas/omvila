using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Randevu okuma (Customers modülü: müşteri kartı randevular sekmesi ve CustomerSummary.nextAppointmentUtc; Notifications:
/// bildirim metni; Sales: kasa satırı). Çağıranın
/// transaction'ında ve markanın kiracı bağlamında çalışır; başka markanın randevusu dönmez.
/// </summary>
public interface IAppointmentQueries
{
    /// <summary>
    /// Kartın randevuları, yeniden eskiye (başlangıç azalan, sonra kimlik). <paramref name="onlyStaffId"/> doluysa yalnız o
    /// personelin kalemi olan randevular (personel rolü "yalnız kendi randevuları"). İmleç bozuksa validation_failed.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="brandCustomerId">Müşteri kartı.</param>
    /// <param name="onlyStaffId">Personel kısıtı.</param>
    /// <param name="limit">Sayfa boyu (1–100).</param>
    /// <param name="cursor">Önceki sayfanın imleci.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result<CustomerAppointmentPage>> ListForCustomerAsync(
        Guid brandId, Guid brandCustomerId, Guid? onlyStaffId, int limit, string? cursor, CancellationToken ct);

    /// <summary>
    /// Kartların yaklaşan ilk randevusu (onay bekleyen veya onaylı, başlangıcı <paramref name="fromUtc"/> veya sonrası).
    /// Tek sorgu; randevusu olmayan kart sonuçta yer almaz.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="brandCustomerIds">Kartlar.</param>
    /// <param name="fromUtc">Şimdi.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, Instant>> GetNextAppointmentsAsync(
        Guid brandId, IReadOnlyCollection<Guid> brandCustomerIds, Instant fromUtc, CancellationToken ct);

    /// <summary>Randevuların başlangıcı (paket seans kullanımlarının görünümü); markada olmayanlar yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentIds">Randevular.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, Instant>> GetStartTimesAsync(Guid brandId, IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct);

    /// <summary>Bildirim metni için randevu bilgisi; randevu markada yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentId">Randevu.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<AppointmentNotificationInfo?> GetNotificationInfoAsync(Guid brandId, Guid appointmentId, CancellationToken ct);

    /// <summary>Tahsilat satırları için randevu bilgisi (toplu, tek sorgu); markada olmayan randevu sonuçta yer almaz.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentIds">Randevular.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, AppointmentPaymentContext>> GetPaymentContextsAsync(
        Guid brandId, IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct);

    /// <summary>
    /// Ödemelerim "Salon onayı bekleniyor" satırları (sözleşme meListPayments, MePaymentStatus AwaitingConfirmation): müşterinin bütün
    /// markalarda "Havaleyi yaptım" dediği, salonun henüz "Kapora geldi" demediği ve hâlâ onay bekleyen randevular. Platform
    /// bağlamında yalnız global müşteri kimliğiyle süzülür; oturumun önceki bağlamı geri yüklenir.
    /// </summary>
    /// <param name="customerId">Global müşteri.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<ReportedDepositInfo>> ListReportedDepositsAsync(Guid customerId, CancellationToken ct);
}
