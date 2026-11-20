namespace Omvila.Modules.Identity.Contracts;

/// <summary>Personel listesinde gösterilen üyelik özeti.</summary>
/// <param name="Role">Owner, BranchManager, Staff.</param>
/// <param name="Status">Invited ("Davet gönderildi"), Active, Disabled.</param>
/// <param name="PhoneMasked">0532 *** ** 05.</param>
public sealed record MembershipInfo(string Role, string Status, string PhoneMasked);
