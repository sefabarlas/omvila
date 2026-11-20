namespace Omvila.Modules.Catalog.Domain;

/// <summary>Bir randevu kalemi için kapora çözümünün girdileri.</summary>
internal sealed record DepositContext(
    bool HasActiveBankAccount,
    bool IsOnlineChannel,
    bool IsPackageRedemption,
    BrandDepositPolicy BrandPolicy,
    int CustomerNoShowCount,
    int RiskyCustomerNoShowThreshold);
