namespace Omvila.Modules.Sales.Domain;

/// <summary>Paket içeriği satırı.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="SessionCount">Seans sayısı (1–100).</param>
internal sealed record PackageDraftItem(Guid ServiceId, int SessionCount);
