using Microsoft.AspNetCore.Authorization;

namespace Omvila.Infrastructure.Security;

internal sealed record AdminPermissionRequirement(string Permission) : IAuthorizationRequirement;
