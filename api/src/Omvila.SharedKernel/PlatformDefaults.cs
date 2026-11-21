namespace Omvila.SharedKernel;

/// <summary>
/// Platform düzeyindeki adlandırılmış varsayılanlar. Her biri docs/alan/sureler-ve-ayarlar.md'deki satıra
/// karşılık gelir; değer değişirse önce o tablo, sonra bu sınıf değişir (AGENTS.md § 5: sabit değer yok).
/// Ortama göre değişen teknik ayarlar (süreler, anahtarlar) appsettings'tedir.
/// </summary>
public static class PlatformDefaults
{
    /// <summary>sureler-ve-ayarlar.md § Abonelik ve platform: SlugRedirectDays.</summary>
    public const int SlugRedirectDays = 30;

    /// <summary>Şube saat dilimi varsayılanı (AGENTS.md § 5, ADR-0003, dilim-01 § Alan kuralları).</summary>
    public const string DefaultTimeZoneId = "Europe/Istanbul";

    /// <summary>Kayıtta oluşan ilk şubenin adı (dilim-01 § Alan kuralları: ad "Merkez").</summary>
    public const string FirstBranchName = "Merkez";

    /// <summary>Para birimi varsayılanı (alan-modeli.md § Brand.DefaultCurrencyCode).</summary>
    public const Currency DefaultCurrency = Currency.TRY;

    /// <summary>sureler-ve-ayarlar.md § Randevu kuralları: CheckInEarliestMinutes ("Geldi" en erken başlangıçtan 60 dk önce; A10).</summary>
    public const int CheckInEarliestMinutes = 60;

    /// <summary>sureler-ve-ayarlar.md § Randevu kuralları: RecurringHorizonMonths (tekrarlayan randevuda somut üretilen ufuk).</summary>
    public const int RecurringHorizonMonths = 3;

    /// <summary>sureler-ve-ayarlar.md § Kimlik ve güvenlik: AccessCodeValidityAfterEndDays (SMS bağlantısı randevu bitişinden 7 gün sonra geçersiz).</summary>
    public const int AccessCodeValidityAfterEndDays = 7;

    /// <summary>musaitlik.md § Çok kalemli randevu: v1'de aynı randevuda en fazla 5 kalem (sözleşme items maxItems 5).</summary>
    public const int MaxItemsPerAppointment = 5;

    /// <summary>musaitlik.md § Katman 2: randevu yazma kilidinin zaman aşımı (lock_timeout = 5s); aşılırsa 503 try_again.</summary>
    public const int AppointmentLockTimeoutSeconds = 5;
}
