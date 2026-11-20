using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Personele özel fark (alan-modeli.md § ServiceStaff): "Keratin 75 dk (özel)", "Balyaj +300 ₺".</summary>
internal sealed record StaffServiceOverride(int? DurationOverrideMinutes, Money? Surcharge);
