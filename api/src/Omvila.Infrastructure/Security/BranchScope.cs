using System.Security.Claims;

namespace Omvila.Infrastructure.Security;

/// <summary>
/// Jetondaki şube kapsamı (roller-yetkiler.md: Ş = kendi şubeleri). <see cref="All"/> = bütün şubeler (Owner).
/// Kapsam dışı şube 404 ile yanıtlanır (varlık sızdırılmaz).
/// </summary>
public sealed class BranchScope
{
    private readonly HashSet<Guid> _branchIds;

    private BranchScope(bool all, IEnumerable<Guid> branchIds)
    {
        All = all;
        _branchIds = [.. branchIds];
    }

    public bool All { get; }

    public IReadOnlyCollection<Guid> BranchIds => _branchIds;

    public static BranchScope From(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        var values = principal.FindAll(OmvilaClaims.BranchIds).Select(c => c.Value).ToList();
        if (values.Contains(OmvilaClaims.AllBranches, StringComparer.Ordinal))
        {
            return new BranchScope(all: true, []);
        }

        return new BranchScope(all: false, values.Select(v => Guid.TryParse(v, out var id) ? id : Guid.Empty).Where(id => id != Guid.Empty));
    }

    public bool Includes(Guid branchId) => All || _branchIds.Contains(branchId);
}
