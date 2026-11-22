using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Features.Users;

/// <summary>Sözleşme RoleMatrix.</summary>
internal sealed record RoleMatrixResponse(IReadOnlyList<RoleMatrixRow> Rows);
