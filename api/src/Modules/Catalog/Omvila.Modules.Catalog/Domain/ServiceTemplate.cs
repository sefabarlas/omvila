namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Sektörün hazır hizmeti (alan-modeli.md § Industry, ServiceTemplate). Platform verisidir: markaya ait değildir,
/// içerik docs/urun/hazir-hizmet-listeleri.md ile migration tohumundan gelir.
/// </summary>
internal sealed class ServiceTemplate
{
    private ServiceTemplate()
    {
    }

    public Guid Id { get; private set; }

    public string IndustryCode { get; private set; } = string.Empty;

    public string CategoryName { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public int DefaultDurationMinutes { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }
}
