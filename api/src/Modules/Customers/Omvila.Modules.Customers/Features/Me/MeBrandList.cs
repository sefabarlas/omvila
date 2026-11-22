namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeBrandList.</summary>
/// <param name="Items">Salonlar.</param>
internal sealed record MeBrandList(IReadOnlyList<MeBrand> Items);
