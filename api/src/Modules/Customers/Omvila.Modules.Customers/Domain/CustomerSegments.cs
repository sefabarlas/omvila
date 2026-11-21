using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Hesaplanan müşteri durumları ve sistem etiketleri (sozluk.md § Müşteri listesi filtreleri; alan-modeli.md § Tag:
/// sistem etiketleri saklanmaz, hesaplanır). Tanımlar sözlükteki kural tablosundandır; marka ayarı değildir.
/// </summary>
internal static class CustomerSegments
{
    /// <summary>"Yeni": ilk ziyaret son 30 günde veya hiç tamamlanan randevusu yok.</summary>
    public const int NewCustomerDays = Contracts.CustomerDisplayInfo.NewCustomerDays;

    /// <summary>"Düzenli": son 180 günde en az 3 tamamlanan randevu.</summary>
    public const int RegularWindowDays = 180;

    public const int RegularMinVisits = 3;

    /// <summary>"60+ gündür gelmeyen".</summary>
    public const int LapsedDays = 60;

    public const string RegularTag = "Düzenli";
    public const string NewTag = "Yeni";

    public static bool IsNew(CustomerActivity a, LocalDate today) =>
        a.VisitCount == 0 || (a.FirstVisitOn is { } first && first >= today.PlusDays(-NewCustomerDays));

    /// <summary>
    /// Yaklaşık hesap (T-46): ziyaret tarihleri Scheduling'de olduğundan "son 180 günde ≥ 3" yerine toplam ziyaret ≥ 3
    /// ve son ziyaret son 180 gün içinde kullanılır. Scheduling gelince randevu geçmişinden kesin hesaplanmalı.
    /// </summary>
    public static bool IsRegular(CustomerActivity a, LocalDate today) =>
        a.VisitCount >= RegularMinVisits && a.LastVisitOn is { } last && last >= today.PlusDays(-RegularWindowDays);

    /// <summary>"Riskli müşteri" türetilmiş durumdur: NoShowCount ≥ BrandSettings.RiskyCustomerNoShowThreshold.</summary>
    public static bool IsNoShowRisk(CustomerActivity a, int riskyThreshold) => a.NoShowCount >= riskyThreshold;

    /// <summary>
    /// Son ziyaret 60 günden eski. "Yaklaşan randevu yok" koşulu Scheduling gelince eklenecek (T-46).
    /// </summary>
    public static bool IsLapsed(CustomerActivity a, LocalDate today) =>
        a.LastVisitOn is { } last && last < today.PlusDays(-LapsedDays);

    /// <summary>Listedeki etiketler (Musteriler: "Düzenli", "Yeni", "2 kez gelmedi"). "Paketli" Sales modülüyle gelir.</summary>
    public static IReadOnlyList<string> Tags(CustomerActivity a, LocalDate today, int riskyThreshold)
    {
        var tags = new List<string>(3);
        if (IsRegular(a, today))
        {
            tags.Add(RegularTag);
        }

        if (IsNew(a, today))
        {
            tags.Add(NewTag);
        }

        if (IsNoShowRisk(a, riskyThreshold))
        {
            tags.Add($"{a.NoShowCount} kez gelmedi");
        }

        return tags;
    }
}
