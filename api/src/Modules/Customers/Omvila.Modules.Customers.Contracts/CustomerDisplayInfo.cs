using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Randevu ekranlarında müşteri satırı ("Derya Şahin · 0532 *** ** 18").</summary>
/// <param name="BrandCustomerId">Markadaki müşteri kartı.</param>
/// <param name="CustomerId">Global kişi.</param>
/// <param name="FullName">Salonun gördüğü ad (kartın adı, yoksa kişinin adı).</param>
/// <param name="PhoneE164">
/// Tam numara. Bildirim göndermek içindir; personele gösterilip gösterilmeyeceğine çağıran karar verir. Kapıda sıradan telefonsuz
/// başlatılan müşteride boştur (T-86 W6).
/// </param>
/// <param name="PhoneMasked">Maskeli gösterim; numara anonimleştirildiyse null.</param>
/// <param name="NoShowCount">Gelmeme sayısı.</param>
/// <param name="IsBlocked">Engelli mi.</param>
/// <param name="AppointmentSmsEnabled">"Randevu bilgilendirme SMS'leri gönderilsin" tercihi.</param>
/// <param name="VisitCount">Tamamlanan ziyaret sayısı.</param>
/// <param name="FirstVisitOn">İlk ziyaret günü.</param>
/// <param name="LastVisitOn">Son ziyaret günü.</param>
public sealed record CustomerDisplayInfo(
    Guid BrandCustomerId,
    Guid CustomerId,
    string FullName,
    string? PhoneE164,
    string? PhoneMasked,
    int NoShowCount,
    bool IsBlocked,
    bool AppointmentSmsEnabled,
    int VisitCount,
    LocalDate? FirstVisitOn,
    LocalDate? LastVisitOn)
{
    /// <summary>sozluk.md § Müşteri listesi filtreleri: "Yeni" = ilk ziyaret son 30 günde veya hiç tamamlanan randevusu yok.</summary>
    public const int NewCustomerDays = 30;

    /// <summary>"Yeni müşteri" (takvim bloğu ve yan panel rozeti); <paramref name="today"/> şubenin yerel günüdür.</summary>
    /// <param name="today">Bugün.</param>
    public bool IsNewOn(LocalDate today) =>
        VisitCount == 0 || (FirstVisitOn is { } first && first >= today.PlusDays(-NewCustomerDays));
}
