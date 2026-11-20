namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Abonelik planı (alan-modeli.md § Plan). Platform verisidir, markaya ait değildir. Fiyat ve sınırlar S-14 çözülene
/// kadar boş olabilir: boş fiyat "belirlenmedi", boş sınır "sınırsız" demektir.
/// </summary>
internal sealed class Plan
{
    private Plan()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public long? MonthlyPriceMinor { get; private set; }

    public long? YearlyPriceMinor { get; private set; }

    public int? IncludedBranchCount { get; private set; }

    public long? ExtraBranchPriceMinor { get; private set; }

    public int? StaffLimit { get; private set; }

    public int? IncludedSmsCredits { get; private set; }

    public bool IsPublic { get; private set; }

    public bool IsActive { get; private set; }

    public static Plan Unpriced(Guid id, string code, string name) => new()
    {
        Id = id,
        Code = code,
        Name = name,
        IsPublic = false,
        IsActive = true,
    };
}
