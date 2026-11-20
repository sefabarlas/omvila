namespace Omvila.Modules.Identity.Features.ManageSession;

/// <summary>Sözleşme ManageSession.</summary>
internal sealed record ManageSessionResponse(
    SessionUser User,
    SessionBrand Brand,
    SessionMembership Membership,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<SessionBranch> Branches,
    SessionSubscription Subscription);
