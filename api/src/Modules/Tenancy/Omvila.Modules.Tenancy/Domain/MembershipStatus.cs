namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Üyelik durumu (durum-makinesi.md: Invited → Active → Disabled; "Davet gönderildi" rozeti).</summary>
internal enum MembershipStatus
{
    Invited,
    Active,
    Disabled,
}
