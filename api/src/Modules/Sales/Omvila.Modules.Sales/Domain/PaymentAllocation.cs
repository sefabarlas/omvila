namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Bir tahsilatın hedefe ayrılan payı (alan-modeli.md § PaymentAllocation). Bir randevu birden çok tahsilatla kapanabilir
/// (kapora havalesi + salonda nakit). Hedef başka modülün kaydıdır; yabancı anahtar yoktur.
/// </summary>
internal sealed class PaymentAllocation
{
    private PaymentAllocation()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid PaymentId { get; private set; }

    public AllocationTargetType TargetType { get; private set; }

    public Guid TargetId { get; private set; }

    public long AmountMinor { get; private set; }

    internal static PaymentAllocation Create(Guid id, Guid brandId, Guid paymentId, AllocationTargetType targetType, Guid targetId, long amountMinor) =>
        new()
        {
            Id = id,
            BrandId = brandId,
            PaymentId = paymentId,
            TargetType = targetType,
            TargetId = targetId,
            AmountMinor = amountMinor,
        };
}
