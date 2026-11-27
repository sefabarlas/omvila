namespace Omvila.Modules.Platform.DemoData;

/// <summary>Sonuç: abonelik durumu, kesilen fatura numarası (fiyat yoksa null), bildirim ve banka hareketi sayısı.</summary>
public sealed record DemoSubscriptionResult(string Status, string? InvoiceNumber, Guid PaymentNoticeId, int BankTransactions);
