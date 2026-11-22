using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Havale bildirimi (alan-modeli.md § PaymentNotice). Salon "Havaleyi bildir" ile yazar; platform ekibi banka hareketiyle eşleştirir,
/// bulamaz veya reddeder. Beklenen tutar bildirim anındaki tutardır (fiyat yoksa boş). Dekont dosya deposunda, burada yalnız anahtarı.
/// </summary>
internal sealed class PaymentNotice
{
    public const int SenderNameMaxLength = 120;
    public const int ReceiptKeyMaxLength = 200;
    public const int ReceiptContentTypeMaxLength = 40;

    private PaymentNotice()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public PaymentNoticePurpose Purpose { get; private set; }

    public Guid? CreditPackageId { get; private set; }

    /// <summary>Kontör bildiriminde paketin kontörü (bildirim anındaki).</summary>
    public int? Credits { get; private set; }

    /// <summary>Abonelikte ödenen dönemin başı.</summary>
    public LocalDate? PeriodStart { get; private set; }

    public long? ExpectedAmountMinor { get; private set; }

    public PaymentNoticeChannel Channel { get; private set; }

    public string TransferCode { get; private set; } = string.Empty;

    public string? SenderName { get; private set; }

    public string? ReceiptFileKey { get; private set; }

    public string? ReceiptContentType { get; private set; }

    public Guid? NotifiedByUserId { get; private set; }

    public Instant NotifiedUtc { get; private set; }

    public PaymentNoticeStatus Status { get; private set; }

    public Guid? MatchedBankTransactionId { get; private set; }

    public Guid? ResolvedByUserId { get; private set; }

    public Instant? ResolvedUtc { get; private set; }

    /// <summary>Abonelik havalesi: ödenen dönem denemede deneme bitişinden başlayan ilk dönem, diğer durumlarda geçerli dönem.</summary>
    public static PaymentNotice ForSubscription(
        Guid id, Subscription subscription, long? expectedAmountMinor, PaymentNoticeChannel channel, string? senderName, Guid? userId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        var periodStart = subscription.Status == SubscriptionStatus.Trialing ? subscription.NextPeriod().Start : subscription.CurrentPeriodStart;
        return new PaymentNotice
        {
            Id = id,
            BrandId = subscription.BrandId,
            Purpose = PaymentNoticePurpose.Subscription,
            PeriodStart = periodStart,
            ExpectedAmountMinor = expectedAmountMinor,
            Channel = channel,
            TransferCode = subscription.TransferCode,
            SenderName = Clean(senderName),
            NotifiedByUserId = userId,
            NotifiedUtc = now,
            Status = PaymentNoticeStatus.Pending,
        };
    }

    public static PaymentNotice ForCredits(
        Guid id, Subscription subscription, CreditPackage package, PaymentNoticeChannel channel, string? senderName, Guid? userId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        ArgumentNullException.ThrowIfNull(package);
        return new PaymentNotice
        {
            Id = id,
            BrandId = subscription.BrandId,
            Purpose = PaymentNoticePurpose.Credits,
            CreditPackageId = package.Id,
            Credits = package.Credits,
            ExpectedAmountMinor = package.PriceMinor,
            Channel = channel,
            TransferCode = subscription.CreditTransferCode,
            SenderName = Clean(senderName),
            NotifiedByUserId = userId,
            NotifiedUtc = now,
            Status = PaymentNoticeStatus.Pending,
        };
    }

    public void AttachReceipt(string fileKey, string contentType)
    {
        if (Status != PaymentNoticeStatus.Pending || string.IsNullOrWhiteSpace(fileKey))
        {
            throw new DomainException("Dekont yalnız bekleyen bildirime eklenir.");
        }

        ReceiptFileKey = fileKey;
        ReceiptContentType = contentType;
    }

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → Matched)
    public Result Match(Guid bankTransactionId, Guid resolvedBy, Instant now) =>
        Resolve(PaymentNoticeStatus.Matched, resolvedBy, now, bankTransactionId);

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → NotFound, "Bulunamadı, markaya yaz")
    public Result MarkNotFound(Guid resolvedBy, Instant now) => Resolve(PaymentNoticeStatus.NotFound, resolvedBy, now, null);

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → Rejected)
    public Result Reject(Guid resolvedBy, Instant now) => Resolve(PaymentNoticeStatus.Rejected, resolvedBy, now, null);

    internal static string? Clean(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private Result Resolve(PaymentNoticeStatus status, Guid resolvedBy, Instant now, Guid? bankTransactionId)
    {
        if (Status != PaymentNoticeStatus.Pending)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = status;
        ResolvedByUserId = resolvedBy;
        ResolvedUtc = now;
        MatchedBankTransactionId = bankTransactionId;
        return Result.Success();
    }
}
