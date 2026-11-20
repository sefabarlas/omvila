namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// Transaction'a uygulanan kiracı bağlamı (ADR-0006 § Kiracı bağlamı nereden gelir).
/// Marka kimliği yalnız jetondan, slug'dan veya erişim kodundan çözülür; istekten okunmaz.
/// </summary>
public sealed record TenantScope
{
    private TenantScope(Guid? brandId, Guid? customerId, bool isPlatform)
    {
        BrandId = brandId;
        CustomerId = customerId;
        IsPlatform = isPlatform;
    }

    /// <summary>Hiçbir markanın satırı görünmez (kimlik doğrulama öncesi, anonim).</summary>
    public static TenantScope None { get; } = new(null, null, isPlatform: false);

    /// <summary>Platform bağlamı (admin yüzeyi, marka ötesi tarayan işler).</summary>
    public static TenantScope Platform { get; } = new(null, null, isPlatform: true);

    public Guid? BrandId { get; }

    public Guid? CustomerId { get; }

    public bool IsPlatform { get; }

    public static TenantScope ForBrand(Guid brandId) => new(brandId, null, isPlatform: false);

    public static TenantScope ForCustomer(Guid customerId) => new(null, customerId, isPlatform: false);
}
