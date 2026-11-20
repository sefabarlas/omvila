using Microsoft.AspNetCore.Authorization;

namespace Omvila.Infrastructure.Security;

internal sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
