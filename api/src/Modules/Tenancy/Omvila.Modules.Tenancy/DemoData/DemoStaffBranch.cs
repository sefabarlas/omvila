namespace Omvila.Modules.Tenancy.DemoData;

/// <summary>Personelin bir şubedeki saatleri; ilk şube birincildir.</summary>
public sealed record DemoStaffBranch(Guid BranchId, IReadOnlyList<DemoHours> Hours);
