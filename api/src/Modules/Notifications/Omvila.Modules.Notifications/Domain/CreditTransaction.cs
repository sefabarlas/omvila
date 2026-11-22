using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>Kontör defteri satırı (alan-modeli.md § CreditTransaction). Değişmez: bakiye negatif olamaz; satırlar güncellenmez.</summary>
internal sealed class CreditTransaction
{
    /// <summary>Düşüm ve iadenin kaynak türü (notification_outbox satırı).</summary>
    public const string NotificationReferenceType = "Notification";

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

    /// <summary>
    /// Satın alma (alan-modeli.md § PaymentNotice "kontörde CreditTransaction(Purchase) yazılır"): havale eşleşince paketin
    /// kontörü eklenir; birim fiyat paket fiyatından (tanımlı değilse boş).
    /// </summary>
    public static CreditTransaction Purchase(
        Guid id, Guid brandId, int credits, int balanceBefore, long? unitPriceMinor, string referenceType, Guid referenceId, Instant now)
    {
        if (credits <= 0)
        {
            throw new DomainException("Satın alınan kontör sıfırdan büyük olmalıdır.");
        }

        return new CreditTransaction
        {
            Id = id,
            BrandId = brandId,
            Type = CreditTransactionType.Purchase,
            Amount = credits,
            BalanceAfter = checked(balanceBefore + credits),
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            UnitPriceMinor = unitPriceMinor,
            CreatedUtc = now,
        };
    }

    /// <summary>SMS gönderimi düşümü (bildirimler.md § Kontör). Bakiye yetmiyorsa çağrılmaz; CHECK kısıtı son savunmadır.</summary>
    public static CreditTransaction Consume(Guid id, Guid brandId, int credits, int balanceBefore, Guid notificationId, Instant now)
    {
        if (credits <= 0)
        {
            throw new DomainException("Düşülen kontör sıfırdan büyük olmalıdır.");
        }

        if (balanceBefore < credits)
        {
            throw new DomainException("Kontör bakiyesi negatif olamaz.");
        }

        return new CreditTransaction
        {
            Id = id,
            BrandId = brandId,
            Type = CreditTransactionType.Consume,
            Amount = -credits,
            BalanceAfter = balanceBefore - credits,
            ReferenceType = NotificationReferenceType,
            ReferenceId = notificationId,
            CreatedUtc = now,
        };
    }

    /// <summary>Teslim edilemeyen SMS'in kontörü iade edilir (T-64).</summary>
    public static CreditTransaction Refund(Guid id, Guid brandId, int credits, int balanceBefore, Guid notificationId, Instant now)
    {
        if (credits <= 0)
        {
            throw new DomainException("İade edilen kontör sıfırdan büyük olmalıdır.");
        }

        return new CreditTransaction
        {
            Id = id,
            BrandId = brandId,
            Type = CreditTransactionType.Refund,
            Amount = credits,
            BalanceAfter = checked(balanceBefore + credits),
            ReferenceType = NotificationReferenceType,
            ReferenceId = notificationId,
            CreatedUtc = now,
        };
    }
}
