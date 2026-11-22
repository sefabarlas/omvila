namespace Omvila.Modules.Platform.Domain;

/// <summary>Abonelik ödeme yolu (sözleşme SubscriptionPaymentMethod). Card yalnız kart sağlayıcısı seçilince (T-06).</summary>
internal enum SubscriptionPaymentMethod
{
    Card,
    BankTransfer,
}
