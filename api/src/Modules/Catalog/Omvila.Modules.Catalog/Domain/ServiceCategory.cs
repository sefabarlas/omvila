using NodaTime;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Hizmet kategorisi (alan-modeli.md § ServiceCategory). Marka içinde ad tektir.</summary>
internal sealed class ServiceCategory
{
    private ServiceCategory()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public static ServiceCategory Create(Guid id, Guid brandId, string name, int sortOrder, Instant now) => new()
    {
        Id = id,
        BrandId = brandId,
        Name = name.Trim(),
        SortOrder = sortOrder,
        IsActive = true,
        CreatedUtc = now,
    };

    public void Update(string name, int sortOrder, bool isActive)
    {
        Name = name.Trim();
        SortOrder = sortOrder;
        IsActive = isActive;
    }
}
