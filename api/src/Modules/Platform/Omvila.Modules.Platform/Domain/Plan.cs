namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Abonelik planı (alan-modeli.md § Plan). Platform verisidir, markaya ait değildir. Fiyat ve sınırlar S-14 çözülene
/// kadar boş olabilir: boş fiyat "belirlenmedi", boş sınır "sınırsız" demektir.
/// </summary>
internal sealed class Plan
{
    public const int NameMaxLength = 60;
    public const int DescriptionMaxLength = 120;

    private Plan()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    /// <summary>Kısa açıklama ("Küçük ve orta salonlar").</summary>
    public string? Description { get; private set; }

    public PlanFeatures Features { get; private set; } = PlanFeatures.None;

    public long? MonthlyPriceMinor { get; private set; }

    public long? YearlyPriceMinor { get; private set; }

    public int? IncludedBranchCount { get; private set; }

    public long? ExtraBranchPriceMinor { get; private set; }

    public int? StaffLimit { get; private set; }

    public int? IncludedSmsCredits { get; private set; }

    public bool IsPublic { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>Satır sürümü (xmin; If-Match).</summary>
    public uint Version { get; private set; }

    public static Plan Unpriced(Guid id, string code, string name) => new()
    {
        Id = id,
        Code = code,
        Name = name,
        IsPublic = false,
        IsActive = true,
    };

    /// <summary>
    /// "Planı düzenle" (adminUpdatePlans): ad, açıklama, fiyat, sınır ve özellikler. Kod değişmez; mevcut abonelikler korunur (fiyat
    /// yeni kayıtlarda ve sonraki faturalarda geçerlidir). Doğrulama uçtadır.
    /// </summary>
    public void Update(PlanValues values)
    {
        ArgumentNullException.ThrowIfNull(values);
        Name = values.Name.Trim();
        Description = PaymentNotice.Clean(values.Description);
        MonthlyPriceMinor = values.MonthlyPriceMinor;
        YearlyPriceMinor = values.YearlyPriceMinor;
        IncludedBranchCount = values.IncludedBranchCount;
        ExtraBranchPriceMinor = values.ExtraBranchPriceMinor;
        StaffLimit = values.StaffLimit;
        IncludedSmsCredits = values.IncludedSmsCredits;
        Features = values.Features;
        IsPublic = values.IsPublic;
        IsActive = values.IsActive;
    }

    /// <summary>Düzenlenebilir değerler (denetim kaydında önce/sonra karşılaştırması için de kullanılır).</summary>
    public PlanValues Values() => new(
        Name, Description, MonthlyPriceMinor, YearlyPriceMinor, IncludedBranchCount, ExtraBranchPriceMinor, StaffLimit, IncludedSmsCredits, Features,
        IsPublic, IsActive);
}
