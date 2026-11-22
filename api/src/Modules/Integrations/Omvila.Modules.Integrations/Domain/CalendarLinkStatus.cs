namespace Omvila.Modules.Integrations.Domain;

/// <summary>CalendarLink.Status (durum-makinesi.md § CalendarLink).</summary>
internal enum CalendarLinkStatus
{
    Active,
    TokenExpired,
    Revoked,
}
