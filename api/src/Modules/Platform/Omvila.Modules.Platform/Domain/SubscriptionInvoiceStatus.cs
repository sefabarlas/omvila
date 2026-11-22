namespace Omvila.Modules.Platform.Domain;

/// <summary>Fatura durumu (sözleşme SubscriptionInvoiceStatus; alan-modeli.md § SubscriptionInvoice).</summary>
internal enum SubscriptionInvoiceStatus
{
    Issued,
    Paid,
    Failed,
    Void,
}
