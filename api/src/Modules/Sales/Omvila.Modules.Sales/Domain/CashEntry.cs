namespace Omvila.Modules.Sales.Domain;

/// <summary>Kasa toplamına giren bir tahsilat (yöntem, tutar, kapora mı, durum).</summary>
internal readonly record struct CashEntry(PaymentMethod Method, long AmountMinor, bool IsDeposit, PaymentStatus Status);
