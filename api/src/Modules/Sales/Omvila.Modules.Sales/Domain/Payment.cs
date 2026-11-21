using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Tahsilat kaydı (alan-modeli.md § Payment). OMVILA para almaz: bu, salonun kendi tahsilatının kaydıdır. Kapora havalesi
/// ("Kapora geldi", durum-makinesi.md A3), "Tamamla ve ödeme al" satırları (A11) ve iade işareti ("Salon iade etti", V7;
/// <see cref="RefundedAmountMinor"/>, <see cref="PaymentStatus.Refunded"/> / <see cref="PaymentStatus.PartiallyRefunded"/>).
/// </summary>
internal sealed class Payment
{
    public const int ReferenceMaxLength = 100;

    private readonly List<PaymentAllocation> _allocations = [];

    private Payment()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid? BrandCustomerId { get; private set; }

    public PaymentMethod Method { get; private set; }

    public long AmountMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    /// <summary>Kapora havalesi.</summary>
    public bool IsDeposit { get; private set; }

    public PaymentStatus Status { get; private set; }

    /// <summary>Salonun iade ettiğini işaretlediği tutar (≤ <see cref="AmountMinor"/>).</summary>
    public long RefundedAmountMinor { get; private set; }

    /// <summary>İadenin işaretlendiği an; kasada iade bu anın şube yerel gününe düşülür (W3).</summary>
    public Instant? RefundedUtc { get; private set; }

    public Instant PaidUtc { get; private set; }

    public Guid? CollectedByUserId { get; private set; }

    /// <summary>Havale açıklama kodu vb.</summary>
    public string? Reference { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyList<PaymentAllocation> Allocations => _allocations;

    /// <summary>
    /// Kapora havalesi: <c>Method = Transfer</c>, <c>IsDeposit</c>, <c>Status = Recorded</c>, tutarın tamamı randevuya tahsis edilir.
    /// </summary>
    // Kural: durum-makinesi.md A3 (Payment(Method=Transfer, IsDeposit) + tahsis)
    public static Payment RecordDeposit(
        IIdGenerator ids, Guid brandId, Guid branchId, Guid brandCustomerId, Guid appointmentId, Money amount, string? reference, Guid? userId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (amount.AmountMinor <= 0)
        {
            throw new DomainException("Kapora tutarı sıfırdan büyük olmalı.");
        }

        var cleaned = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
        if (cleaned is { Length: > ReferenceMaxLength })
        {
            throw new DomainException("Tahsilat referansı çok uzun.");
        }

        var payment = new Payment
        {
            Id = ids.New(),
            BrandId = brandId,
            BranchId = branchId,
            BrandCustomerId = brandCustomerId,
            Method = PaymentMethod.Transfer,
            AmountMinor = amount.AmountMinor,
            CurrencyCode = amount.Currency.ToString(),
            IsDeposit = true,
            Status = PaymentStatus.Recorded,
            RefundedAmountMinor = 0,
            PaidUtc = now,
            CollectedByUserId = userId,
            Reference = cleaned,
        };
        payment._allocations.Add(PaymentAllocation.Create(ids.New(), brandId, payment.Id, AllocationTargetType.Appointment, appointmentId, amount.AmountMinor));
        return payment;
    }

    /// <summary>
    /// "Tamamla ve ödeme al" satırı (A11): salonda alınan nakit, salonun kendi POS'u (Card) veya havale. <c>IsDeposit = false</c>,
    /// <c>Status = Recorded</c>, tutarın tamamı randevuya tahsis edilir. Paketten düşüm (PackageRedemption) bu yolla yazılmaz.
    /// </summary>
    // Kural: durum-makinesi.md A11 ("tahsilatlar kaydedilir"); alan-modeli.md § PaymentAllocation
    public static Payment RecordCollected(
        IIdGenerator ids, Guid brandId, Guid branchId, Guid brandCustomerId, Guid appointmentId, PaymentMethod method, Money amount, Guid? userId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (method is not (PaymentMethod.Cash or PaymentMethod.Card or PaymentMethod.Transfer))
        {
            throw new DomainException("Salonda alınan tahsilat nakit, kart veya havale olmalı.");
        }

        if (amount.AmountMinor <= 0)
        {
            throw new DomainException("Tahsilat tutarı sıfırdan büyük olmalı.");
        }

        var payment = new Payment
        {
            Id = ids.New(),
            BrandId = brandId,
            BranchId = branchId,
            BrandCustomerId = brandCustomerId,
            Method = method,
            AmountMinor = amount.AmountMinor,
            CurrencyCode = amount.Currency.ToString(),
            IsDeposit = false,
            Status = PaymentStatus.Recorded,
            RefundedAmountMinor = 0,
            PaidUtc = now,
            CollectedByUserId = userId,
        };
        payment._allocations.Add(PaymentAllocation.Create(ids.New(), brandId, payment.Id, AllocationTargetType.Appointment, appointmentId, amount.AmountMinor));
        return payment;
    }

    /// <summary>
    /// Paket satışı (manageSellPackage): salonda alınan nakit, salonun kendi POS'u veya havale; tutar paket fiyatıdır ve tamamı
    /// müşteri paketine tahsis edilir (Kasa'da "Fön paketi · 5 seans").
    /// </summary>
    // Kural: alan-modeli.md § PaymentAllocation (TargetType CustomerPackage); sözleşme manageSellPackage
    public static Payment RecordPackageSale(
        IIdGenerator ids, Guid brandId, Guid branchId, Guid brandCustomerId, Guid customerPackageId, PaymentMethod method, Money amount, Guid? userId, Instant now)
    {
        var payment = RecordCollected(ids, brandId, branchId, brandCustomerId, customerPackageId, method, amount, userId, now);
        payment._allocations.Clear();
        payment._allocations.Add(PaymentAllocation.Create(
            ids.New(), brandId, payment.Id, AllocationTargetType.CustomerPackage, customerPackageId, amount.AmountMinor));
        return payment;
    }

    /// <summary>
    /// Paketten düşülen kalemin 0 TL'lik kaydı (W1): <c>Method = PackageRedemption</c>, tutar 0, randevuya 0 tahsis. Kasada
    /// tutar toplamına girmez; randevunun ödemeleri arasında "Paketten" görünür.
    /// </summary>
    // Kural: docs/acik-sorular.md W1, T-86 (1)
    public static Payment RecordPackageRedemption(
        IIdGenerator ids, Guid brandId, Guid branchId, Guid brandCustomerId, Guid appointmentId, Currency currency, Guid? userId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        var payment = new Payment
        {
            Id = ids.New(),
            BrandId = brandId,
            BranchId = branchId,
            BrandCustomerId = brandCustomerId,
            Method = PaymentMethod.PackageRedemption,
            AmountMinor = 0,
            CurrencyCode = currency.ToString(),
            IsDeposit = false,
            Status = PaymentStatus.Recorded,
            RefundedAmountMinor = 0,
            PaidUtc = now,
            CollectedByUserId = userId,
        };
        payment._allocations.Add(PaymentAllocation.Create(ids.New(), brandId, payment.Id, AllocationTargetType.Appointment, appointmentId, 0));
        return payment;
    }

    /// <summary>
    /// "Salon iade etti" (V7) ve paket iadesi (S-10, W9): salonun kendi yaptığı iadenin işareti, para hareketi değildir. Tek kez ve yalnız
    /// <c>Recorded</c> kayıtta; tutarın tamamıysa <c>Refunded</c>, azsa <c>PartiallyRefunded</c>. Çağıran (randevu kuralı)
    /// tutarı ve tek seferliği önceden denetler; buraya aykırı girdi gelmesi programcı hatasıdır.
    /// </summary>
    // Kural: durum-makinesi.md § 6 (Recorded → PartiallyRefunded / Refunded); V7
    public void MarkRefunded(Money amount, Instant now)
    {
        if (Status != PaymentStatus.Recorded)
        {
            throw new DomainException("Yalnız kayıtlı tahsilat iade işaretlenir.");
        }

        if (!string.Equals(amount.Currency.ToString(), CurrencyCode, StringComparison.Ordinal) || amount.AmountMinor <= 0 || amount.AmountMinor > AmountMinor)
        {
            throw new DomainException("İade tutarı tahsilat tutarını aşamaz ve sıfırdan büyük olmalı.");
        }

        RefundedAmountMinor = amount.AmountMinor;
        RefundedUtc = now;
        Status = amount.AmountMinor == AmountMinor ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
    }
}
