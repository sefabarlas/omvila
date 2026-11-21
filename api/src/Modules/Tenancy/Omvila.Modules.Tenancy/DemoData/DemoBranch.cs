namespace Omvila.Modules.Tenancy.DemoData;

/// <summary>Demo şubesi: ad, adres ve haftalık saatler.</summary>
public sealed record DemoBranch(string Name, string AddressLine, string District, string City, IReadOnlyList<DemoHours> Hours);
