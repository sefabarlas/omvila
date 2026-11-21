using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Domain.Import;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Imports;

/// <summary>
/// CSV içe aktarma (MusteriIceAktar; sözleşme managePreviewImport, manageCommitImport). Önizleme hiçbir müşteri kaydını
/// değiştirmez; geçerli satırları içe aktarma işinde saklar. Uygulama aynı satırları o anki duruma göre yazar: markada kayıtlı
/// numara "birleşir" (mevcut değerler korunur, yalnız boş not ve doğum yılı dolar), diğerleri <c>Source = Import</c> ile
/// eklenir; global kişi varsa yeniden kullanılır. Kampanya izni yazılmaz. <c>sendInfoSms</c> yeni kartların randevu
/// bilgilendirme SMS tercihidir (birleşen kartın tercihi değişmez; T-92).
/// </summary>
internal sealed class CustomerImporter(
    CustomersDbContext db, IOptions<CustomerImportOptions> options, IIdGenerator ids, IClock clock, IAuditLog audit)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public async Task<Result<CustomerImportPreview>> PreviewAsync(Guid brandId, IFormFile? file, int currentYear, AuditActor actor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(actor);
        var limits = options.Value;
        if (file is null || file.Length == 0)
        {
            return Error.Validation("file", "required");
        }

        if (file.Length > limits.MaxFileBytes)
        {
            return Error.Validation("file", "too_large");
        }

        string text;
        try
        {
            using var reader = new StreamReader(file.OpenReadStream(), StrictUtf8, detectEncodingFromByteOrderMarks: false);
            text = await reader.ReadToEndAsync(ct);
        }
        catch (DecoderFallbackException)
        {
            return Error.Validation("file", "invalid_format");
        }

        var parsed = CustomerImportParser.Parse(text, limits.MaxRows, currentYear);
        if (!parsed.IsSuccess)
        {
            return parsed.Error;
        }

        var import = parsed.Value;
        var registered = await RegisteredPhonesAsync(brandId, [.. import.Rows.Select(r => (string?)r.PhoneE164)], ct);
        var toMerge = import.Rows.Count(r => registered.Contains(r.PhoneE164));
        var preview = CustomerImport.Preview(
            ids.New(), brandId, Path.GetFileName(file.FileName), import.TotalRows, import.Rows.Count - toMerge, toMerge, import.InvalidRows,
            string.Join(',', import.MappedColumns), JsonSerializer.Serialize(import.Errors, Json), JsonSerializer.Serialize(import.Rows, Json),
            actor.UserId, clock.GetCurrentInstant(), Duration.FromMinutes(limits.PreviewTtlMinutes));
        db.CustomerImports.Add(preview);
        await db.SaveChangesAsync(ct);
        return ToPreview(preview, import.Errors);
    }

    public async Task<Result<CustomerImportResult>> CommitAsync(Guid brandId, Guid importId, CustomerImportCommitRequest? body, AuditActor actor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(actor);
        var import = await db.CustomerImports.SingleOrDefaultAsync(i => i.Id == importId && i.BrandId == brandId, ct);
        if (import is null)
        {
            return Error.NotFound();
        }

        // Kendiliğinden idempotent: uygulanmış önizleme aynı sonucu döner.
        if (import.Status == CustomerImportStatus.Committed)
        {
            return ToResult(import);
        }

        var now = clock.GetCurrentInstant();
        var committable = import.EnsureCommittable(now);
        if (!committable.IsSuccess)
        {
            return committable.Error;
        }

        var sendInfoSms = body?.SendInfoSms ?? false;
        var rows = JsonSerializer.Deserialize<List<ImportRow>>(import.RowsJson ?? "[]", Json) ?? [];
        var (created, merged) = await ApplyAsync(brandId, rows, sendInfoSms, now, ct);
        import.MarkCommitted(created, merged, sendInfoSms, now);
        await db.SaveChangesAsync(ct);
        await audit.WriteAsync(
            actor.Entry(brandId, AuditAction.Create, nameof(CustomerImport), import.Id, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["created"] = created,
                ["merged"] = merged,
                ["skipped"] = import.InvalidRows,
                ["sendInfoSms"] = sendInfoSms,
            }),
            ct);
        return ToResult(import);
    }

    private async Task<(int Created, int Merged)> ApplyAsync(Guid brandId, List<ImportRow> rows, bool sendInfoSms, Instant now, CancellationToken ct)
    {
        var phones = rows.Select(r => (string?)r.PhoneE164).ToList();
        var customers = await db.Customers.Where(c => c.PhoneE164 != null && phones.Contains(c.PhoneE164)).ToDictionaryAsync(c => c.PhoneE164!, StringComparer.Ordinal, ct);
        var customerIds = customers.Values.Select(c => c.Id).ToList();
        var cards = await db.BrandCustomers.Where(b => b.BrandId == brandId && customerIds.Contains(b.CustomerId)).ToListAsync(ct);
        var mergedTargets = cards.Where(c => c.MergedIntoId is not null).Select(c => c.MergedIntoId!.Value).ToList();
        var targets = await db.BrandCustomers.Where(b => mergedTargets.Contains(b.Id)).ToDictionaryAsync(b => b.Id, ct);
        int created = 0, merged = 0;
        foreach (var row in rows)
        {
            var card = customers.TryGetValue(row.PhoneE164, out var customer) ? cards.Find(c => c.CustomerId == customer.Id) : null;
            while (card?.MergedIntoId is { } targetId)
            {
                card = targets.GetValueOrDefault(targetId) ?? await db.BrandCustomers.SingleOrDefaultAsync(b => b.Id == targetId, ct);
            }

            if (card is not null)
            {
                card.FillBlanks(row.Notes, row.BirthYear);
                merged++;
                continue;
            }

            if (customer is null)
            {
                customer = Customer.Create(ids.New(), Phone(row.PhoneE164), row.FullName, birthYear: null, isVerified: false, now);
                db.Customers.Add(customer);
                customers[row.PhoneE164] = customer;
            }

            var newCard = BrandCustomer.Create(ids.New(), brandId, customer.Id, row.FullName, row.BirthYear, row.Notes, BrandCustomerSource.Import, sendInfoSms, now);
            db.BrandCustomers.Add(newCard);
            cards.Add(newCard);
            created++;
        }

        await db.SaveChangesAsync(ct);
        return (created, merged);
    }

    /// <summary>Markada (birleşmiş kartlar kalan karta giderek) kartı olan numaralar; anonim kartlar gerçek numarayı taşımaz.</summary>
    private async Task<HashSet<string>> RegisteredPhonesAsync(Guid brandId, List<string?> phones, CancellationToken ct) =>
        [.. await (from b in db.BrandCustomers.AsNoTracking()
                   join c in db.Customers.AsNoTracking() on b.CustomerId equals c.Id
                   where b.BrandId == brandId && c.PhoneE164 != null && phones.Contains(c.PhoneE164)
                   select c.PhoneE164!).ToListAsync(ct)];

    private static PhoneNumber Phone(string e164) =>
        PhoneNumber.TryParse(e164, out var phone) ? phone : throw new DomainException("Önizlemedeki telefon çözülemedi.");

    private static CustomerImportPreview ToPreview(CustomerImport import, IReadOnlyList<ImportRowError> errors) => new(
        import.Id, import.FileName, import.TotalRows, import.ToCreate, import.ToMerge, import.InvalidRows,
        import.MappedColumns.Split(',', StringSplitOptions.RemoveEmptyEntries), errors, import.Status.ToString(), import.ExpiresUtc);

    private static CustomerImportResult ToResult(CustomerImport import) =>
        new(import.Id, import.Status.ToString(), import.Created, import.Merged, import.InvalidRows);
}
