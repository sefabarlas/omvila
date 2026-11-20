namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Şube kimliği ve adı (sözleşme BranchRef).</summary>
/// <param name="Id">Şube.</param>
/// <param name="Name">Şube adı.</param>
public sealed record BranchSummary(Guid Id, string Name);
