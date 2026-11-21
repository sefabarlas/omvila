namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Personelin iki şube arasındaki geçiş süresi (BranchTransferTime, yönlü).</summary>
/// <param name="FromBranchId">Çıkış şubesi.</param>
/// <param name="ToBranchId">Varış şubesi.</param>
/// <param name="Minutes">Dakika.</param>
public sealed record BranchTransfer(Guid FromBranchId, Guid ToBranchId, int Minutes);
