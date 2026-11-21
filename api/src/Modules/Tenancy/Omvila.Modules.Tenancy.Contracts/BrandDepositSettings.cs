namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Kapora çözümünün marka ayarları (sureler-ve-ayarlar.md § Kapora).</summary>
/// <param name="DepositPolicy">None, RiskyCustomersOnly, Always.</param>
/// <param name="RiskyCustomerNoShowThreshold">"Riskli müşteri tanımı: N kez gelmeyen" (1–10).</param>
/// <param name="HasActiveBankAccount">Aktif kapora hesabı (IBAN) var mı; yoksa kapora istenmez.</param>
public sealed record BrandDepositSettings(string DepositPolicy, int RiskyCustomerNoShowThreshold, bool HasActiveBankAccount);
