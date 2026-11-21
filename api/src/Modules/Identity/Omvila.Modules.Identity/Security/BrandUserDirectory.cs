using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// İzin matrisine (<see cref="RolePermissions"/>) göre bildirim alıcıları. Kapsamı "Ş" olan izinde kullanıcının şube
/// kümesi olayın şubesini içermelidir; "T" kapsamı, Owner ve boş küme bütün şubelerdir.
/// </summary>
internal sealed class BrandUserDirectory(IdentityDbContext db) : IBrandUserDirectory
{
    public async Task<IReadOnlyList<Guid>> GetUserIdsWithPermissionAsync(Guid brandId, string permission, Guid? branchId, CancellationToken ct)
    {
        var members = await (from m in db.Memberships.AsNoTracking()
                             join u in db.Users.AsNoTracking() on m.UserId equals u.Id
                             where m.BrandId == brandId && m.Status == MembershipStatus.Active && u.IsActive
                             orderby m.CreatedUtc
                             select new { m.UserId, m.Role, m.BranchIds }).ToListAsync(ct);
        return [.. members
            .Where(m => RolePermissions.ByRole.TryGetValue(m.Role, out var granted) && granted.TryGetValue(permission, out var scope)
                && (branchId is null || scope == PermissionScope.All || m.BranchIds.Length == 0
                    || string.Equals(m.Role, Roles.Owner, StringComparison.Ordinal) || m.BranchIds.Contains(branchId.Value)))
            .Select(m => m.UserId)
            .Distinct()];
    }
}
