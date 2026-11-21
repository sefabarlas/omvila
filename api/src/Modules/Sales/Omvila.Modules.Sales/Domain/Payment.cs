using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Tahsilat kaydı (alan-modeli.md § Payment). OMVILA para almaz: bu, salonun kendi tahsilatının kaydıdır. Dilim 4'te yalnız
/// kapora havalesi yazılır ("Kapora geldi", durum-makinesi.md A3); iade işareti (<see cref="RefundedAmountMinor"/>,
/// <see cref="PaymentStatus.Refunded"/> / <see cref="PaymentStatus.PartiallyRefunded"/>) sonraki parçadadır.
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
}
