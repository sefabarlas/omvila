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
    public const int ReceiptFileNameMaxLength = 120;
    public const int ResolutionMessageMaxLength = 300;

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

    /// <summary>Dekontun yüklenirken verilen adı ("dekont-ziraat.pdf"); gösterim içindir, dosya anahtarı değildir.</summary>
    public string? ReceiptFileName { get; private set; }

    public long? ReceiptSizeBytes { get; private set; }

    public Guid? NotifiedByUserId { get; private set; }

    public Instant NotifiedUtc { get; private set; }

    public PaymentNoticeStatus Status { get; private set; }

    public Guid? MatchedBankTransactionId { get; private set; }

    public Guid? ResolvedByUserId { get; private set; }

    public Instant? ResolvedUtc { get; private set; }

    /// <summary>Platform ekibinin markaya notu (NotFound, Rejected).</summary>
    public string? ResolutionMessage { get; private set; }

    /// <summary>Satır sürümü (xmin; If-Match).</summary>
    public uint Version { get; private set; }

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

    public void AttachReceipt(string fileKey, string contentType, string? fileName = null, long? sizeBytes = null)
    {
        if (Status != PaymentNoticeStatus.Pending || string.IsNullOrWhiteSpace(fileKey))
        {
            throw new DomainException("Dekont yalnız bekleyen bildirime eklenir.");
        }

        ReceiptFileKey = fileKey;
        ReceiptContentType = contentType;
        ReceiptFileName = Clean(fileName) is { } name ? (name.Length > ReceiptFileNameMaxLength ? name[^ReceiptFileNameMaxLength..] : name) : null;
        ReceiptSizeBytes = sizeBytes;
    }

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → Matched)
    public Result Match(Guid bankTransactionId, Guid resolvedBy, Instant now) =>
        Resolve(PaymentNoticeStatus.Matched, resolvedBy, now, bankTransactionId);

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → NotFound, "Bulunamadı, markaya yaz")
    public Result MarkNotFound(Guid resolvedBy, Instant now, string? message = null) =>
        Resolve(PaymentNoticeStatus.NotFound, resolvedBy, now, null, message);

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → Rejected)
    public Result Reject(Guid resolvedBy, Instant now, string? message = null) =>
        Resolve(PaymentNoticeStatus.Rejected, resolvedBy, now, null, message);

    internal static string? Clean(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private Result Resolve(PaymentNoticeStatus status, Guid resolvedBy, Instant now, Guid? bankTransactionId, string? message = null)
    {
        if (Status != PaymentNoticeStatus.Pending)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = status;
        ResolvedByUserId = resolvedBy;
        ResolvedUtc = now;
        MatchedBankTransactionId = bankTransactionId;
        ResolutionMessage = Clean(message);
        return Result.Success();
    }
}
