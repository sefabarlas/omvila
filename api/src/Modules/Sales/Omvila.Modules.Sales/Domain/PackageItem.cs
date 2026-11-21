namespace Omvila.Modules.Sales.Domain;

/// <summary>Paket tanımının içerik satırı (alan-modeli.md § PackageItem).</summary>
internal sealed class PackageItem
{
    private PackageItem()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid PackageId { get; private set; }

    public int SeqNo { get; private set; }

    public Guid ServiceId { get; private set; }

    public int SessionCount { get; private set; }

    internal static PackageItem Create(Guid id, Guid brandId, Guid packageId, int seqNo, PackageDraftItem draft) =>
        new() { Id = id, BrandId = brandId, PackageId = packageId, SeqNo = seqNo, ServiceId = draft.ServiceId, SessionCount = draft.SessionCount };
}
