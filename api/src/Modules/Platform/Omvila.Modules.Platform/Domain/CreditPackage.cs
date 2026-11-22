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

    public static CreditPackage Unpriced(Guid id, string code, string name, int credits) => new()
    {
        Id = id,
        Code = code,
        Name = name,
        Credits = credits,
        IsActive = true,
    };
}
