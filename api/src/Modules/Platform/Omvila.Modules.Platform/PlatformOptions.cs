namespace Omvila.Modules.Platform;

/// <summary>
/// "Platform" bölümü (sureler-ve-ayarlar.md § Abonelik ve platform). Deneme kontörü, plan değerleri, KDV oranı ve OMVILA hesap
/// bilgisi S-14 gereği yapılandırmadan/veriden okunur; kodda yazılmaz. Boşsa ilgili alan istemciye null döner.
/// </summary>
public sealed class PlatformOptions
{
    public const string Section = "Platform";

    /// <summary>TrialDays.</summary>
    public int TrialDays { get; set; }

    /// <summary>PastDueGraceDays: gecikmeden kaç gün sonra salt okunur.</summary>
    public int PastDueGraceDays { get; set; }

    /// <summary>ReadOnlyToCancelledDays: salt okunurdan kaç gün sonra iptal (durum-makinesi.md § 8 "30 gün"; T-118).</summary>
    public int ReadOnlyToCancelledDays { get; set; }

    /// <summary>CancelledArchiveDays: iptalden kaç gün sonra marka arşive alınır.</summary>
    public int CancelledArchiveDays { get; set; }

    /// <summary>TrialEndingNoticeDays: owner.trial_ending e-postası deneme bitmeden kaç gün önce (bildirimler.md: 3).</summary>
    public int TrialEndingNoticeDays { get; set; }

    /// <summary>AdvanceSubscriptionStatuses turu (dakika). Kurallar gün bazlıdır; saatlik tur yeterlidir.</summary>
    public int AdvanceSubscriptionStatusesIntervalMinutes { get; set; }

    /// <summary>Deneme aboneliğinin bağlandığı plan (Trialing'de her şey açıktır; durum-makinesi.md).</summary>
    public string TrialPlanCode { get; set; } = string.Empty;

    /// <summary>TrialCredits ([DENEME KONTÖRÜ]). Boşsa hediye yazılmaz.</summary>
    public int? TrialCredits { get; set; }

    /// <summary>Fatura KDV oranı (yüzde; S-14). Boşsa vergi 0 yazılır (T-118).</summary>
    public decimal? VatRatePercent { get; set; }

    /// <summary>OmvilaBankAccount ([OMVILA ŞİRKET UNVANI], [OMVILA IBAN]). Boşsa havale bilgisi null döner.</summary>
    public OmvilaBankAccountOptions OmvilaBankAccount { get; set; } = new();

    /// <summary>Yalnız geliştirme ve test: migrate sonrası eklenecek planlar (S-14: fiyatsız, sınırsız).</summary>
    public IList<PlanSeed> SeedPlans { get; } = [];

    /// <summary>Yalnız geliştirme ve test: migrate sonrası eklenecek kontör paketleri (fiyatsız).</summary>
    public IList<CreditPackageSeed> SeedCreditPackages { get; } = [];

    internal bool IsValid() =>
        TrialDays > 0 && PastDueGraceDays > 0 && ReadOnlyToCancelledDays > 0 && CancelledArchiveDays > 0
        && TrialEndingNoticeDays > 0 && TrialEndingNoticeDays < TrialDays && AdvanceSubscriptionStatusesIntervalMinutes > 0
        && !string.IsNullOrWhiteSpace(TrialPlanCode) && TrialCredits is null or > 0
        && VatRatePercent is null or (>= 0 and <= 100)
        && OmvilaBankAccount.IsEmptyOrValid()
        && SeedPlans.All(p => !string.IsNullOrWhiteSpace(p.Code) && !string.IsNullOrWhiteSpace(p.Name))
        && SeedCreditPackages.All(p => !string.IsNullOrWhiteSpace(p.Code) && !string.IsNullOrWhiteSpace(p.Name) && p.Credits > 0);
}
