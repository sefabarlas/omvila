using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Abonelik faturası (alan-modeli.md § SubscriptionInvoice). Bilgi amaçlıdır, e-Arşiv değildir. Numara yıl başına kilitli sayaç
/// satırıyla boşluksuz (<see cref="InvoiceNumbers"/>). Tutar KDV hariç, vergi ayrı; vade dönem başıdır (T-118).
/// </summary>
internal sealed class SubscriptionInvoice
{
    public const int InvoiceNumberMaxLength = 20;

    private SubscriptionInvoice()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid SubscriptionId { get; private set; }

    public string InvoiceNumber { get; private set; } = string.Empty;

    public LocalDate PeriodStart { get; private set; }

    public LocalDate PeriodEnd { get; private set; }

    public long AmountMinor { get; private set; }

    public long TaxMinor { get; private set; }

    public Currency Currency { get; private set; }

    public SubscriptionInvoiceStatus Status { get; private set; }

    public Instant IssuedUtc { get; private set; }

    public LocalDate DueOn { get; private set; }

    public Instant? PaidUtc { get; private set; }

    public SubscriptionPaymentMethod? PaidVia { get; private set; }

    public long TotalMinor => AmountMinor + TaxMinor;

    public static SubscriptionInvoice Issue(
        Guid id, Subscription subscription, string invoiceNumber, LocalDate periodStart, LocalDate periodEnd, InvoiceAmount amount, Instant now)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        ArgumentNullException.ThrowIfNull(amount);
        if (string.IsNullOrWhiteSpace(invoiceNumber) || periodEnd < periodStart)
        {
            throw new DomainException("Fatura numarası boş veya dönem geçersiz.");
        }

        return new SubscriptionInvoice
        {
            Id = id,
            BrandId = subscription.BrandId,
            SubscriptionId = subscription.Id,
            InvoiceNumber = invoiceNumber,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            AmountMinor = amount.NetMinor,
            TaxMinor = amount.TaxMinor,
            Currency = amount.Currency,
            Status = SubscriptionInvoiceStatus.Issued,
            IssuedUtc = now,
            DueOn = periodStart,
        };
    }

    // Kural: alan-modeli.md § PaymentNotice "Eşleştirme sonucu: abonelikte fatura Paid olur"
    public Result MarkPaid(SubscriptionPaymentMethod via, Instant now)
    {
        if (Status is not (SubscriptionInvoiceStatus.Issued or SubscriptionInvoiceStatus.Failed))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = SubscriptionInvoiceStatus.Paid;
        PaidUtc = now;
        PaidVia = via;
        return Result.Success();
    }

    public Result MarkVoid()
    {
        if (Status == SubscriptionInvoiceStatus.Paid)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = SubscriptionInvoiceStatus.Void;
        return Result.Success();
    }
}
