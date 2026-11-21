namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Personele özel hizmet prim oranı (alan-modeli.md § ServiceStaff.CommissionPercentOverride).</summary>
/// <param name="StaffId">Personel.</param>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="Percent">Oran (0–100).</param>
public sealed record StaffServiceCommission(Guid StaffId, Guid ServiceId, decimal Percent);
