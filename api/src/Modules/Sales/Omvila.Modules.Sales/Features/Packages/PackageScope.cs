using Omvila.Infrastructure.Security;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>
/// Paket listelerinin şube kapsamı (roller-yetkiler.md package.sell: şube yöneticisi kendi şubeleri). Şube verilirse o şubede
/// satılanlar ve bütün şubelerde geçerli olanlar; verilmezse kapsamdaki şubelerde satılanlar ve bütün şubelerde geçerli olanlar.
/// </summary>
internal sealed record PackageScope(BranchScope Branches, Guid? BranchId)
{
    /// <summary>Sorgudaki şube kapsamda mı (değilse 404).</summary>
    public bool IsValid => BranchId is not { } branch || Branches.Includes(branch);

    /// <summary>Satıldığı şube kapsamda mı (iade ve tekil işlem; değilse 404).</summary>
    public bool Covers(CustomerPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);
        return Branches.Includes(package.BranchId);
    }

    public IQueryable<CustomerPackage> Apply(IQueryable<CustomerPackage> query)
    {
        if (BranchId is { } branch)
        {
            return query.Where(p => p.BranchId == branch || p.IsAvailableAllBranches);
        }

        if (Branches.All)
        {
            return query;
        }

        var ids = Branches.BranchIds.ToList();
        return query.Where(p => ids.Contains(p.BranchId) || p.IsAvailableAllBranches);
    }
}
