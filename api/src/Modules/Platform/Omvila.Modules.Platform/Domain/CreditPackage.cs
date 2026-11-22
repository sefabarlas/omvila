namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// SMS kontör paketi (alan-modeli.md § CreditPackage). Platform verisidir; fiyat S-14 çözülene kadar boş olabilir ("belirlenmedi").
/// </summary>
internal sealed class CreditPackage
{
    private CreditPackage()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public int Credits { get; private set; }

    public long? PriceMinor { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>Satır sürümü (xmin; If-Match).</summary>
    public uint Version { get; private set; }

    public static CreditPackage Unpriced(Guid id, string code, string name, int credits) => new()
    {
        Id = id,
        Code = code,
        Name = name,
        Credits = credits,
        IsActive = true,
    };

    /// <summary>
    /// Kontör paketini düzenler (adminUpdateCreditPackages). Kod değişmez; bekleyen havale bildirimlerinin beklenen tutarı bildirimde
    /// saklı olduğundan etkilenmez. Doğrulama uçtadır.
    /// </summary>
    public void Update(string name, int credits, long? priceMinor, bool isActive)
    {
        if (credits <= 0 || priceMinor < 0 || string.IsNullOrWhiteSpace(name))
        {
            throw new SharedKernel.DomainException("Kontör paketi değerleri geçersiz.");
        }

        Name = name.Trim();
        Credits = credits;
        PriceMinor = priceMinor;
        IsActive = isActive;
    }
}
