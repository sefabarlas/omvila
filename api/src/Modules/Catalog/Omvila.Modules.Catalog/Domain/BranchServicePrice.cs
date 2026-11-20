namespace Omvila.Modules.Catalog.Domain;

/// <summary>Şubeye özel fiyat veya süre (alan-modeli.md § BranchServicePrice; şube ve hizmet başına bir).</summary>
internal sealed class BranchServicePrice
{
    private BranchServicePrice()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid ServiceId { get; private set; }

    public long PriceMinor { get; private set; }

    public int? DurationOverrideMinutes { get; private set; }

    public bool IsAvailable { get; private set; }

    public static BranchServicePrice Create(Guid id, Guid brandId, Guid branchId, Guid serviceId, BranchPrice price)
    {
        var row = new BranchServicePrice { Id = id, BrandId = brandId, BranchId = branchId, ServiceId = serviceId };
        row.Set(price);
        return row;
    }

    public void Set(BranchPrice price)
    {
        ArgumentNullException.ThrowIfNull(price);
        PriceMinor = price.Price.AmountMinor;
        DurationOverrideMinutes = price.DurationOverrideMinutes;
        IsAvailable = price.IsAvailable;
    }
}
