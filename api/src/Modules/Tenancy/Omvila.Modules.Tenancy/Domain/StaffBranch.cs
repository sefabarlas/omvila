namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Personelin çalıştığı şube (alan-modeli.md § StaffBranch; UNIQUE (StaffId, BranchId)).</summary>
internal sealed class StaffBranch
{
    private StaffBranch()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid StaffId { get; private set; }

    public Guid BranchId { get; private set; }

    public bool IsPrimary { get; private set; }

    public bool IsActive { get; private set; }

    public void SetActive(bool isActive) => IsActive = isActive;

    public static StaffBranch Create(Guid id, Guid brandId, Guid staffId, Guid branchId, bool isPrimary) => new()
    {
        Id = id,
        BrandId = brandId,
        StaffId = staffId,
        BranchId = branchId,
        IsPrimary = isPrimary,
        IsActive = true,
    };
}
