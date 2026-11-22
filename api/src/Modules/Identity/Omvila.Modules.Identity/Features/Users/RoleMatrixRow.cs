using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Features.Users;

/// <summary>Sözleşme RoleMatrix satırı.</summary>
internal sealed record RoleMatrixRow(string Group, IReadOnlyList<string> Permissions, string Owner, string BranchManager, string Staff);
