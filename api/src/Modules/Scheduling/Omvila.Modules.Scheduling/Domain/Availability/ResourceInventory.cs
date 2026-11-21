namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Şubenin kaynakları, bakım dışı aralıkları ve aktif kullanımları (musaitlik.md § 6). Catalog'da kaynak tabloları henüz
/// yok; yükleyici boş envanter verir, kural alan testleriyle güvencededir (T-48).
/// </summary>
internal sealed record ResourceInventory(
    IReadOnlyList<BranchResource> Resources,
    IReadOnlyList<ResourceBlock> Downtimes,
    IReadOnlyList<ResourceBlock> Usages)
{
    public static ResourceInventory None { get; } = new([], [], []);
}
