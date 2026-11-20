namespace Omvila.Modules.Customers.Contracts;

/// <summary>Randevu ekranlarında müşteri satırı ("Derya Şahin · 0532 *** ** 18").</summary>
/// <param name="BrandCustomerId">Markadaki müşteri kartı.</param>
/// <param name="CustomerId">Global kişi.</param>
/// <param name="FullName">Salonun gördüğü ad (kartın adı, yoksa kişinin adı).</param>
/// <param name="PhoneE164">Tam numara. Bildirim göndermek içindir; personele gösterilip gösterilmeyeceğine çağıran karar verir.</param>
/// <param name="PhoneMasked">Maskeli gösterim; numara anonimleştirildiyse null.</param>
/// <param name="NoShowCount">Gelmeme sayısı.</param>
/// <param name="IsBlocked">Engelli mi.</param>
/// <param name="AppointmentSmsEnabled">"Randevu bilgilendirme SMS'leri gönderilsin" tercihi.</param>
public sealed record CustomerDisplayInfo(
    Guid BrandCustomerId,
    Guid CustomerId,
    string FullName,
    string PhoneE164,
    string? PhoneMasked,
    int NoShowCount,
    bool IsBlocked,
    bool AppointmentSmsEnabled);
