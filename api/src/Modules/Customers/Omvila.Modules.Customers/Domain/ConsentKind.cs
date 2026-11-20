namespace Omvila.Modules.Customers.Domain;

/// <summary>Rıza türü (alan-modeli.md § ConsentRecord).</summary>
internal enum ConsentKind
{
    PrivacyNoticeAcknowledged,
    MarketingSms,
    MarketingEmail,
    MarketingPush,
}
