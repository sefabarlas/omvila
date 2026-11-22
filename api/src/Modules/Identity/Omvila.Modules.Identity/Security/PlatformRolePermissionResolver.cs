using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Identity.Security;

internal sealed class PlatformRolePermissionResolver : IAdminPermissionResolver
{
    public bool HasPermission(string platformRole, string permission) =>
        PlatformRolePermissions.ByRole.TryGetValue(platformRole, out var permissions) && permissions.Contains(permission);
}
