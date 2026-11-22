namespace Omvila.Modules.Identity.Features.AdminAuth;

/// <summary>Sözleşme AdminSession.</summary>
internal sealed record AdminSession(AdminSessionUser User, string Role, IReadOnlyList<string> Permissions, int PendingApprovalCount);
