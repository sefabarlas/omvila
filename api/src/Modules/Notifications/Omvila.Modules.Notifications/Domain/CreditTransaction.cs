using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>Kontör defteri satırı (alan-modeli.md § CreditTransaction). Değişmez: bakiye negatif olamaz; satırlar güncellenmez.</summary>
internal sealed class CreditTransaction
{
    private CreditTransaction()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public CreditTransactionType Type { get; private set; }

    public int Amount { get; private set; }

    public int BalanceAfter { get; private set; }

    public string? ReferenceType { get; private set; }

    public Guid? ReferenceId { get; private set; }

    public long? UnitPriceMinor { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public static CreditTransaction Grant(
        Guid id, Guid brandId, int credits, int balanceBefore, string referenceType, Guid? referenceId, Instant now)
    {
        if (credits <= 0)
        {
            throw new DomainException("Hediye kontör sıfırdan büyük olmalıdır.");
        }

        return new CreditTransaction
        {
            Id = id,
            BrandId = brandId,
            Type = CreditTransactionType.Grant,
            Amount = credits,
            BalanceAfter = checked(balanceBefore + credits),
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            CreatedUtc = now,
        };
    }
}
