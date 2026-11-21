using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>"Tamamla ve ödeme al" satırı.</summary>
/// <param name="Method">Yöntem.</param>
/// <param name="Amount">Tutar (sıfırdan büyük, randevunun para biriminde).</param>
public sealed record CompletionPaymentLine(CollectedPaymentMethod Method, Money Amount);
