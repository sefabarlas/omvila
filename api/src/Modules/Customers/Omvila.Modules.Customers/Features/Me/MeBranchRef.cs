namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeBranchRef.</summary>
/// <param name="Id">Şube.</param>
/// <param name="Name">Şube adı.</param>
/// <param name="District">İlçe.</param>
internal sealed record MeBranchRef(Guid Id, string Name, string District);
