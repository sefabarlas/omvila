using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Sözleşme StaffServiceAssignment.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="DurationOverrideMinutes">Personele özel süre.</param>
/// <param name="Surcharge">Personele özel ek ücret.</param>
/// <param name="CommissionPercentOverride">Personele özel prim oranı (0–100).</param>
public sealed record StaffServiceAssignment(Guid ServiceId, int? DurationOverrideMinutes, Money? Surcharge, decimal? CommissionPercentOverride);
