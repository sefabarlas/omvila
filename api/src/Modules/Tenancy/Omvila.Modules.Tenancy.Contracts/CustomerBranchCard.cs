namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Şube özeti (sözleşme MeBranchRef).</summary>
/// <param name="Id">Şube.</param>
/// <param name="Name">Şube adı.</param>
/// <param name="District">İlçe.</param>
public sealed record CustomerBranchCard(Guid Id, string Name, string District);
