namespace Omvila.Modules.Platform;

/// <summary>
/// "Platform" bölümü (sureler-ve-ayarlar.md § Abonelik ve platform). Deneme kontörü ve plan değerleri S-14 gereği
/// yapılandırmadan/veriden okunur; kodda yazılmaz.
/// </summary>
public sealed class PlatformOptions
{
    public const string Section = "Platform";

    /// <summary>TrialDays.</summary>
    public int TrialDays { get; set; }

    /// <summary>Deneme aboneliğinin bağlandığı plan (Trialing'de her şey açıktır; durum-makinesi.md).</summary>
    public string TrialPlanCode { get; set; } = string.Empty;

    /// <summary>TrialCredits ([DENEME KONTÖRÜ]). Boşsa hediye yazılmaz.</summary>
    public int? TrialCredits { get; set; }

    /// <summary>Yalnız geliştirme ve test: migrate sonrası eklenecek planlar (S-14: fiyatsız, sınırsız).</summary>
    public IList<PlanSeed> SeedPlans { get; } = [];

    internal bool IsValid() =>
        TrialDays > 0 && !string.IsNullOrWhiteSpace(TrialPlanCode) && TrialCredits is null or > 0
        && SeedPlans.All(p => !string.IsNullOrWhiteSpace(p.Code) && !string.IsNullOrWhiteSpace(p.Name));
}
