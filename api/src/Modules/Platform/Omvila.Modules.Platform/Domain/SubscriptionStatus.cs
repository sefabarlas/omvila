namespace Omvila.Modules.Platform.Domain;

/// <summary>durum-makinesi.md § Abonelik.</summary>
internal enum SubscriptionStatus
{
    Trialing,
    Active,
    PastDue,
    ReadOnly,
    Cancelled,
}
