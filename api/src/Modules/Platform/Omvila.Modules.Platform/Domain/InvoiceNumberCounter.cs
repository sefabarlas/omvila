namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Yıl başına fatura sayacı (alan-modeli.md § SubscriptionInvoice): satır güncellemesi transaction sonuna kadar kilitli kalır; geri
/// alınan transaction numarayı da geri alır, böylece sıra boşluksuzdur (SEQUENCE boşluk bırakabildiği için kullanılmaz).
/// Yalnız <see cref="Infrastructure.InvoiceNumberAllocator"/> tek SQL ile yazar.
/// </summary>
internal sealed class InvoiceNumberCounter
{
    private InvoiceNumberCounter()
    {
    }

    public int Year { get; private set; }

    public long LastValue { get; private set; }
}
