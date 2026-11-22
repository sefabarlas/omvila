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

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: QuietHoursLocal başlangıcı (22:00, şube yerel).</summary>
    public const int QuietHoursStartHour = 22;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: QuietHoursLocal bitişi (08:00; müşteri hatırlatması bu saate kayar).</summary>
    public const int QuietHoursEndHour = 8;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: DailySummaryLocalTime (20:00, şube yerel; "Her akşam 20:00").</summary>
    public const int DailySummaryHour = 20;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: PushTitleMaxChars (40).</summary>
    public const int PushTitleMaxChars = 40;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: PushBodyMaxChars (90).</summary>
    public const int PushBodyMaxChars = 90;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: LowCreditThreshold 200 (bakiye bunun altına inince "Kontör azaldı").</summary>
    public const int LowCreditThreshold = 200;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: SmsUsageWindowDays 30 ("bu tempoyla ~18 gün yeter" son 30 günün ortalamasıyla; sözleşme SmsOverview).</summary>
    public const int SmsUsageWindowDays = 30;

    /// <summary>sureler-ve-ayarlar.md § Bildirim ve iletişim: ReviewInviteDelayMinutes (değerlendirme daveti tamamlandıktan 120 dk sonra).</summary>
    public const int ReviewInviteDelayMinutes = 120;

    /// <summary>sureler-ve-ayarlar.md § Abonelik ve platform: ReviewPublicThreshold (puan ve "En çok övülen" için en az 5 değerlendirme).</summary>
    public const int ReviewPublicThreshold = 5;

    /// <summary>sureler-ve-ayarlar.md § Abonelik ve platform: ReviewOverviewWindowDays ("Ortalama · son 90 gün", "En çok övülen").</summary>
    public const int ReviewOverviewWindowDays = 90;

    /// <summary>sureler-ve-ayarlar.md § Kapora: TransferCodePrefix varsayılanı ("OMV-7K2X").</summary>
    public const string TransferCodePrefix = "OMV";

    /// <summary>sureler-ve-ayarlar.md § Kapora: TransferWindowMinutes varsayılanı (havale süresi 120 dk).</summary>
    public const int TransferWindowMinutes = 120;

    /// <summary>sureler-ve-ayarlar.md § Bekleme listesi ve kapıda sıra: WaitlistOfferHoldMinutes (boşalan saat 15 dk tutulur).</summary>
    public const int WaitlistOfferHoldMinutes = 15;

    /// <summary>sureler-ve-ayarlar.md § Bekleme listesi ve kapıda sıra: WaitlistMorningEndHour ("Sabah" 12:00'den önce başlayan saat; T-90).</summary>
    public const int WaitlistMorningEndHour = 12;

    /// <summary>sureler-ve-ayarlar.md § Bekleme listesi ve kapıda sıra: WaitlistAfternoonEndHour ("Öğleden sonra" 12:00–17:00, "Akşam" 17:00 ve sonrası; T-90).</summary>
    public const int WaitlistAfternoonEndHour = 17;

    /// <summary>sureler-ve-ayarlar.md § KVKK ve saklama: DeletionRequestDueDays 30 (silme talepleri 30 gün içinde tamamlanır).</summary>
    public const int DeletionRequestDueDays = 30;

    /// <summary>sureler-ve-ayarlar.md § Paketler: PackageExpiringSoonDays 30 ("Süresi yaklaşan": 30 gün içinde biten Active paketler).</summary>
    public const int PackageExpiringSoonDays = 30;
}
