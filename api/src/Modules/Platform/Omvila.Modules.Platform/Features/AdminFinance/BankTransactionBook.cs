using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Text;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminFinance.AdminFinanceViews;

namespace Omvila.Modules.Platform.Features.AdminFinance;

/// <summary>
/// OMVILA hesabına gelen havaleler (sözleşme adminListBankTransactions, adminCreateBankTransaction, adminImportBankTransactions;
/// T-116 (6), T-124). Banka referansı benzersizdir: içe aktarma kendiliğinden idempotenttir (kayıtlı referans duplicate sayılır),
/// elle eklemede aynı referans 409 bank_transaction_duplicate. Giden (eksi) tutarlar alınmaz.
/// </summary>
internal sealed class BankTransactionBook(
    PlatformDbContext db, IPlatformBrandDirectory brands, IIdGenerator ids, IClock clock, IOptions<PlatformOptions> options)
{
    public const int DefaultLimit = 50;
    private const int MaxRangeDays = 92;
    private const int MaxErrors = 100;
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly LocalDatePattern CursorDate = LocalDatePattern.Iso;

    public async Task<Result<BankTransactionPage>> ListAsync(
        string? q, string? fromText, string? toText, bool unmatchedOnly, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = ValidateList(q, fromText, toText, limit, cursor, out var search, out var from, out var to, out var after);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var query = db.BankTransactions.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
        {
            var pattern = $"%{search.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("%", "\\%", StringComparison.Ordinal).Replace("_", "\\_", StringComparison.Ordinal)}%";
            query = query.Where(x => EF.Functions.ILike(x.SenderName, pattern)
                || (x.Description != null && EF.Functions.ILike(x.Description, pattern))
                || EF.Functions.ILike(x.BankReference, pattern));
        }

        if (from is { } lower)
        {
            query = query.Where(x => x.ValueDate >= lower);
        }

        if (to is { } upper)
        {
            query = query.Where(x => x.ValueDate <= upper);
        }

        if (unmatchedOnly)
        {
            query = query.Where(x => x.MatchedNoticeId == null);
        }

        if (after is { } a)
        {
            query = query.Where(x => x.ValueDate < a.Date || (x.ValueDate == a.Date && x.Id.CompareTo(a.Id) < 0));
        }

        var take = limit ?? DefaultLimit;
        var rows = await query.OrderByDescending(x => x.ValueDate).ThenByDescending(x => x.Id).Take(take + 1).ToListAsync(ct);
        var page = rows.Take(take).ToList();
        var views = await ViewsAsync(page, ct);
        return new BankTransactionPage(views, rows.Count > take ? Encode(page[^1].ValueDate, page[^1].Id) : null);
    }

    private static Dictionary<string, string[]> ValidateList(
        string? q, string? fromText, string? toText, int? limit, string? cursor, out string? search, out LocalDate? from, out LocalDate? to,
        out (LocalDate Date, Guid Id)? after)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        from = ParseDate(fromText, "from", errors);
        to = ParseDate(toText, "to", errors);
        search = q?.Trim();
        if (search is { Length: < 2 or > 100 })
        {
            errors["q"] = ["out_of_range"];
        }

        // Kural: sözleşme adminListBankTransactions — tarih aralığı en fazla 92 gün.
        if (from is { } f && to is { } t && (t < f || Period.DaysBetween(f, t) > MaxRangeDays))
        {
            errors["to"] = ["out_of_range"];
        }

        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        after = null;
        if (cursor is not null && !TryDecode(cursor, out after))
        {
            errors["cursor"] = ["invalid_format"];
        }

        return errors;
    }

    public async Task<Result<BankTransactionView>> CreateAsync(BankTransactionCreate body, Guid userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.ValueDate is null)
        {
            errors["valueDate"] = ["required"];
        }

        if (body.Amount?.AmountMinor is not { } amount || body.Amount.Currency is null)
        {
            errors["amount"] = ["required"];
        }
        else if (!string.Equals(body.Amount.Currency, nameof(Currency.TRY), StringComparison.Ordinal))
        {
            errors["amount.currency"] = ["invalid_format"];
        }
        else if (amount <= 0)
        {
            errors["amount.amountMinor"] = ["out_of_range"];
        }

        Check(errors, "senderName", body.SenderName, 2, PlatformBankTransaction.SenderNameMaxLength);
        Check(errors, "bankReference", body.BankReference, 3, PlatformBankTransaction.BankReferenceMaxLength);
        if (body.Description is { Length: > PlatformBankTransaction.DescriptionMaxLength })
        {
            errors["description"] = ["out_of_range"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var reference = body.BankReference!.Trim();
        var existing = await db.BankTransactions.AsNoTracking().Where(x => x.BankReference == reference).Select(x => (Guid?)x.Id).FirstOrDefaultAsync(ct);
        if (existing is { } id)
        {
            return Error.Conflict(
                ErrorCodes.BankTransactionDuplicate, new Dictionary<string, object?>(StringComparer.Ordinal) { ["bankTransactionId"] = id });
        }

        var transaction = PlatformBankTransaction.Create(
            ids.New(), body.ValueDate!.Value, null, body.Amount!.AmountMinor!.Value, body.SenderName!, body.Description, reference,
            BankTransactionSource.Manual, userId, clock.GetCurrentInstant());
        db.BankTransactions.Add(transaction);
        await db.SaveChangesAsync(ct);
        return (await ViewsAsync([transaction], ct))[0];
    }

    /// <summary>CSV önizleme (dryRun = true, varsayılan) veya içe aktarma.</summary>
    public async Task<Result<BankTransactionImportResult>> ImportAsync(IFormCollection form, Guid userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(form);
        var input = await ReadInputAsync(form, ct);
        if (!input.IsSuccess)
        {
            return input.Error;
        }

        var (dryRun, file, parsed) = input.Value;
        var incoming = parsed.Rows.Where(r => r.AmountMinor > 0).ToList();
        var references = incoming.Select(r => r.BankReference).Distinct(StringComparer.Ordinal).ToList();
        var known = (await db.BankTransactions.AsNoTracking().Where(x => references.Contains(x.BankReference)).Select(x => x.BankReference).ToListAsync(ct))
            .ToHashSet(StringComparer.Ordinal);
        var now = clock.GetCurrentInstant();
        var imported = 0;
        var duplicates = 0;
        foreach (var row in incoming)
        {
            // Dosyada aynı referans ikinci kez geçerse o da yinelenen sayılır.
            if (!known.Add(row.BankReference))
            {
                duplicates++;
                continue;
            }

            imported++;
            if (!dryRun)
            {
                db.BankTransactions.Add(PlatformBankTransaction.Create(
                    ids.New(), row.ValueDate, row.ReceivedUtc, row.AmountMinor, row.SenderName, row.Description, row.BankReference,
                    BankTransactionSource.Import, userId, now));
            }
        }

        if (!dryRun)
        {
            await db.SaveChangesAsync(ct);
        }

        return new BankTransactionImportResult(
            dryRun, Path.GetFileName(file.FileName), parsed.TotalRows, imported, duplicates, parsed.Rows.Count - incoming.Count, parsed.Errors.Count,
            [.. parsed.Errors.Take(MaxErrors)]);
    }

    private async Task<Result<(bool DryRun, IFormFile File, BankStatementCsv.Parsed Parsed)>> ReadInputAsync(IFormCollection form, CancellationToken ct)
    {
        var settings = options.Value;
        var dryRunText = form["dryRun"].ToString();
        var dryRun = true;
        if (dryRunText.Length > 0 && !bool.TryParse(dryRunText, out dryRun))
        {
            return Error.Validation("dryRun", "invalid_format");
        }

        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0)
        {
            return Error.Validation("file", "required");
        }

        if (file.Length > settings.BankImportMaxBytes)
        {
            return Error.Validation("file", "too_large");
        }

        var parsed = await ReadAsync(file, ct);
        if (parsed is null)
        {
            return Error.Validation("file", "invalid_format");
        }

        return parsed.TotalRows > settings.BankImportMaxRows ? Error.Validation("file", "too_large") : (dryRun, file, parsed);
    }

    private static async Task<BankStatementCsv.Parsed?> ReadAsync(IFormFile file, CancellationToken ct)
    {
        var extension = Path.GetExtension(file.FileName);
        if (!string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase) && !string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        try
        {
            await using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, StrictUtf8, detectEncodingFromByteOrderMarks: false);
            var text = await reader.ReadToEndAsync(ct);
            return text.Contains('\0', StringComparison.Ordinal)
                ? null
                : BankStatementCsv.Parse(text, DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId]);
        }
        catch (DecoderFallbackException)
        {
            return null;
        }
    }

    private async Task<List<BankTransactionView>> ViewsAsync(IReadOnlyCollection<PlatformBankTransaction> rows, CancellationToken ct)
    {
        var noticeIds = rows.Where(r => r.MatchedNoticeId is not null).Select(r => r.MatchedNoticeId!.Value).ToList();
        var noticeBrands = noticeIds.Count == 0
            ? []
            : await db.PaymentNotices.AsNoTracking().Where(n => noticeIds.Contains(n.Id)).ToDictionaryAsync(n => n.Id, n => n.BrandId, ct);
        var names = noticeBrands.Count == 0 ? [] : (await brands.ListAsync(ct)).ToDictionary(b => b.Id, b => b.Name);
        return [.. rows.Select(r => new BankTransactionView(
            r.Id, r.ValueDate, r.ReceivedUtc, Money.Try(r.AmountMinor), r.SenderName, r.Description, r.BankReference, r.Source, r.MatchedNoticeId,
            r.MatchedNoticeId is { } n && noticeBrands.TryGetValue(n, out var brandId) ? names.GetValueOrDefault(brandId) : null))];
    }

    private static LocalDate? ParseDate(string? text, string field, Dictionary<string, string[]> errors)
    {
        if (text is null)
        {
            return null;
        }

        var parsed = CursorDate.Parse(text);
        if (parsed.Success)
        {
            return parsed.Value;
        }

        errors[field] = ["invalid_format"];
        return null;
    }

    private static void Check(Dictionary<string, string[]> errors, string field, string? value, int min, int max)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            errors[field] = ["required"];
        }
        else if (trimmed.Length < min || trimmed.Length > max)
        {
            errors[field] = ["out_of_range"];
        }
    }

    private static string Encode(LocalDate date, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CursorDate.Format(date)}|{id:N}"));

    private static bool TryDecode(string cursor, out (LocalDate Date, Guid Id)? value)
    {
        value = null;
        try
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(cursor)).Split('|');
            if (parts.Length == 2 && CursorDate.Parse(parts[0]) is { Success: true } date && Guid.TryParseExact(parts[1], "N", out var id))
            {
                value = (date.Value, id);
                return true;
            }
        }
        catch (FormatException)
        {
        }

        return false;
    }
}
