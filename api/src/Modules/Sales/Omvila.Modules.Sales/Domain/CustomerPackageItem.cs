namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Müşteri paketinin hizmet satırı (alan-modeli.md § CustomerPackageItem). Kalan seans sayaç değildir, defterden hesaplanır
/// (<see cref="PackageSessionUsage"/>).
/// </summary>
internal sealed class CustomerPackageItem
{
    private CustomerPackageItem()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid CustomerPackageId { get; private set; }

    public int SeqNo { get; private set; }

    public Guid ServiceId { get; private set; }

    public int TotalSessions { get; private set; }

    internal static CustomerPackageItem Create(Guid id, Guid brandId, Guid customerPackageId, int seqNo, Guid serviceId, int totalSessions) =>
        new() { Id = id, BrandId = brandId, CustomerPackageId = customerPackageId, SeqNo = seqNo, ServiceId = serviceId, TotalSessions = totalSessions };

    /// <summary>Kalan seans: toplam − geri dönmemiş kullanımlar.</summary>
    // Kural: alan-modeli.md § CustomerPackage ("TotalSessions − count(usage WHERE RevertedUtc IS NULL)")
    public int Remaining(int activeUsages) => Math.Max(0, TotalSessions - activeUsages);
}
