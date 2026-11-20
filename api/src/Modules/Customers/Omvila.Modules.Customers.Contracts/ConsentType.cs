namespace Omvila.Modules.Customers.Contracts;

/// <summary>Rıza türü (alan-modeli.md § ConsentRecord).</summary>
public enum ConsentType
{
    /// <summary>Aydınlatma metni okundu.</summary>
    PrivacyNoticeAcknowledged,

    /// <summary>Kampanya SMS izni.</summary>
    MarketingSms,

    /// <summary>Kampanya e-posta izni.</summary>
    MarketingEmail,

    /// <summary>Kampanya bildirim izni.</summary>
    MarketingPush,
}
