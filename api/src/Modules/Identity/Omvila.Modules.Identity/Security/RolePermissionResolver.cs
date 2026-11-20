using Omvila.Infrastructure.Security;

namespace Omvila.Modules.Identity.Security;

internal sealed class RolePermissionResolver : IPermissionResolver
{
    public PermissionScope? Resolve(string role, string permission) =>
        RolePermissions.ByRole.TryGetValue(role, out var permissions) && permissions.TryGetValue(permission, out var scope)
            ? scope
            : null;
}
