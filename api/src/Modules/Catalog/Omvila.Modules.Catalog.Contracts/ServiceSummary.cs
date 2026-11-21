namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Hizmet özeti.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="Name">Ad.</param>
/// <param name="IsActive">Etkin mi (yeni pakete yalnız etkin hizmet girer).</param>
public sealed record ServiceSummary(Guid ServiceId, string Name, bool IsActive);
