using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Files;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>
/// "Havaleyi bildir" (manageNotifyTransfer; dilim-06 kabul kriteri 2). Çok parçalı form: purpose, creditPackageId (kontörde),
/// senderName, receipt (isteğe bağlı dekont ≤ 5 MB, PDF/JPG/PNG; biçim içerikten anlaşılır, dosya adına ve başlığa güvenilmez).
/// Salt okunur ve gecikmede de açıktır (ödemenin yolu budur). Aynı amaç için bekleyen bildirim varsa 409.
/// </summary>
internal sealed class PaymentNoticeIntake(
    PlatformDbContext db, SubscriptionBilling billing, IFileStorage files, IIdGenerator ids, IClock clock)
{
    public const string ReceiptArea = "payment-notices";

    public async Task<Result<PaymentNoticeSummary>> NotifyAsync(
        Guid brandId, IFormCollection form, PaymentNoticeChannel channel, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(form);
        var input = Validate(form);
        if (!input.IsSuccess)
        {
            return input.Error;
        }

        var (purpose, packageId, senderName, receipt, receiptType) = input.Value;
        var subscription = await db.Subscriptions.AsNoTracking().SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        CreditPackage? package = null;
        if (purpose == PaymentNoticePurpose.Credits)
        {
            package = await db.CreditPackages.AsNoTracking().SingleOrDefaultAsync(p => p.Id == packageId && p.IsActive, ct);
        }

        if (subscription is null || (purpose == PaymentNoticePurpose.Credits && package is null))
        {
            return Error.NotFound();
        }

        // Kural: sözleşme manageNotifyTransfer (aynı amaç, kontörde aynı paket için tek bekleyen bildirim)
        var pendingId = await db.PaymentNotices.AsNoTracking()
            .Where(n => n.BrandId == brandId && n.Purpose == purpose && n.CreditPackageId == packageId && n.Status == PaymentNoticeStatus.Pending)
            .Select(n => (Guid?)n.Id)
            .FirstOrDefaultAsync(ct);
        if (pendingId is { } existing)
        {
            return Error.Conflict(
                ErrorCodes.PaymentNoticeAlreadyPending, new Dictionary<string, object?>(StringComparer.Ordinal) { ["paymentNoticeId"] = existing });
        }

        var now = clock.GetCurrentInstant();
        var notice = package is null
            ? PaymentNotice.ForSubscription(
                ids.New(), subscription, (await billing.AmountAsync(subscription, ct))?.TotalMinor, channel, senderName, userId, now)
            : PaymentNotice.ForCredits(ids.New(), subscription, package, channel, senderName, userId, now);
        if (receipt is not null)
        {
            await using var content = receipt.OpenReadStream();
            var key = await files.SaveAsync(ReceiptArea, notice.Id, receiptType!.Value.Extension, content, ct);
            notice.AttachReceipt(key, receiptType.Value.ContentType, Path.GetFileName(receipt.FileName), receipt.Length);
        }

        db.PaymentNotices.Add(notice);
        await db.SaveChangesAsync(ct);
        return PaymentNoticeSummary.From(notice);
    }

    private static Result<(PaymentNoticePurpose Purpose, Guid? PackageId, string? SenderName, IFormFile? Receipt, ReceiptType? Type)> Validate(
        IFormCollection form)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        PaymentNoticePurpose purpose = default;
        var purposeText = form["purpose"].ToString();
        if (purposeText.Length == 0)
        {
            errors["purpose"] = ["required"];
        }
        else if (!char.IsAsciiLetter(purposeText[0]) || !Enum.TryParse(purposeText, ignoreCase: false, out purpose))
        {
            errors["purpose"] = ["invalid_format"];
        }

        Guid? packageId = null;
        var packageText = form["creditPackageId"].ToString();
        if (purpose == PaymentNoticePurpose.Credits && !errors.ContainsKey("purpose"))
        {
            if (packageText.Length == 0)
            {
                errors["creditPackageId"] = ["required"];
            }
            else if (Guid.TryParse(packageText, out var parsed))
            {
                packageId = parsed;
            }
            else
            {
                errors["creditPackageId"] = ["invalid_format"];
            }
        }

        var senderName = PaymentNotice.Clean(form["senderName"].ToString());
        if (senderName is { Length: > PaymentNotice.SenderNameMaxLength })
        {
            errors["senderName"] = ["too_long"];
        }

        var receipt = form.Files.GetFile("receipt");
        ReceiptType? type = null;
        if (receipt is not null)
        {
            if (receipt.Length > PlatformDefaults.PaymentReceiptMaxBytes)
            {
                errors["receipt"] = ["too_large"];
            }
            else if ((type = ReceiptType.Detect(receipt)) is null)
            {
                errors["receipt"] = ["invalid_format"];
            }
        }

        return errors.Count > 0 ? Error.Validation(errors) : (purpose, packageId, senderName, receipt, type);
    }
}
