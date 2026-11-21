using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>
/// Müşteri listesi ve kartı okuma (Musteriler, MusteriKarti). Yalnız markanın kartları; global kişiden yalnız telefon.
/// Birleştirmeyle arşivlenen kart hiç görünmez (404); anonimleştirilen kart listede ve aramada yoktur, kartı "Silinmiş müşteri" olarak açılır.
/// </summary>
internal sealed class CustomerReader(CustomersDbContext db)
{
    public const int DefaultLimit = 50;
    public const int MaxLimit = 100;
    public const int QueryMaxLength = 80;
    private const int LastDigitsSearchLength = 4;
    private const int MinPartialPhoneDigits = 5;

    public IQueryable<CustomerRow> Rows(Guid brandId) =>
        from b in db.BrandCustomers.AsNoTracking()
        join c in db.Customers.AsNoTracking() on b.CustomerId equals c.Id
        where b.BrandId == brandId && b.MergedIntoId == null
        select new CustomerRow
        {
            Id = b.Id,
            IsAnonymized = b.AnonymizedUtc != null,
            FullName = b.DisplayName ?? c.FullName,
            PhoneE164 = c.PhoneE164,
            BirthYear = b.BirthYear,
            VisitCount = b.VisitCount,
            NoShowCount = b.NoShowCount,
            FirstVisitOn = b.FirstVisitOn,
            LastVisitOn = b.LastVisitOn,
            TotalSpentMinor = b.TotalSpentMinor,
            Notes = b.Notes,
            Source = b.Source,
        };

    /// <summary>Listede görünen kartlar: birleşmiş (arşiv) ve anonimleştirilmiş kartlar hariç (T-92).</summary>
    public IQueryable<CustomerRow> ListedRows(Guid brandId) => Rows(brandId).Where(r => !r.IsAnonymized);

    public Task<CustomerRow?> FindAsync(Guid brandId, Guid brandCustomerId, CancellationToken ct) =>
        Rows(brandId).SingleOrDefaultAsync(r => r.Id == brandCustomerId, ct);

    public Task<bool> ExistsAsync(Guid brandId, Guid brandCustomerId, CancellationToken ct) =>
        db.BrandCustomers.AnyAsync(b => b.Id == brandCustomerId && b.BrandId == brandId && b.MergedIntoId == null, ct);

    public async Task<Result<CustomerList>> ListAsync(
        Guid brandId, string? q, string? filter, int? limit, string? cursor, CustomerViewContext view, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var term = q?.Trim();
        if (term is { Length: > QueryMaxLength })
        {
            errors["q"] = ["too_long"];
        }

        var parsedFilter = CustomerListFilter.All;
        if (filter is not null && (!Enum.TryParse(filter, ignoreCase: false, out parsedFilter) || !Enum.IsDefined(parsedFilter)))
        {
            errors["filter"] = ["invalid_format"];
        }

        var take = limit ?? DefaultLimit;
        if (take is < 1 or > MaxLimit)
        {
            errors["limit"] = ["out_of_range"];
        }

        CustomerCursor? after = null;
        if (cursor is not null)
        {
            if (CustomerCursor.TryDecode(cursor, out var decoded))
            {
                after = decoded;
            }
            else
            {
                errors["cursor"] = ["invalid_format"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        // "Paketi olan": paketler Sales modülüyle gelecek (T-46); o zamana kadar hiçbir müşteri bu filtreye girmez.
        if (parsedFilter == CustomerListFilter.WithPackage)
        {
            return new CustomerList([], null);
        }

        var query = Filter(Search(ListedRows(brandId), term), parsedFilter, view);
        if (after is { } a)
        {
            query = After(query, a);
        }

        var rows = await query
            .OrderBy(r => r.LastVisitOn == null)
            .ThenByDescending(r => r.LastVisitOn)
            .ThenByDescending(r => r.Id)
            .Take(take + 1)
            .ToListAsync(ct);
        var next = rows.Count > take ? new CustomerCursor(rows[take - 1].LastVisitOn, rows[take - 1].Id).Encode() : null;
        return new CustomerList([.. rows.Take(take).Select(r => r.ToResponse(view))], next);
    }

    /// <summary>
    /// Arama (api-kurallari.md § Liste: "Ad ya da telefonun son 4 hanesiyle"): rakamsa telefon (tam numara, 4 hane = son 4
    /// hane, 5+ hane = numaranın bir parçası), değilse ad içinde büyük/küçük harf duyarsız arama.
    /// </summary>
    internal static IQueryable<CustomerRow> Search(IQueryable<CustomerRow> rows, string? term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return rows;
        }

        var digits = new string([.. term.Where(char.IsAsciiDigit)]);
        var looksLikePhone = digits.Length > 0 && term.All(ch => char.IsAsciiDigit(ch) || ch is ' ' or '-' or '(' or ')' or '+' or '.');
        if (!looksLikePhone)
        {
            var pattern = $"%{EscapeLike(term)}%";
            return rows.Where(r => EF.Functions.ILike(r.FullName, pattern, @"\"));
        }

        if (PhoneNumber.TryParse(term, out var phone))
        {
            return rows.Where(r => r.PhoneE164 == phone.E164);
        }

        if (digits.Length == LastDigitsSearchLength)
        {
            return rows.Where(r => r.PhoneE164 != null && r.PhoneE164.EndsWith(digits));
        }

        if (digits.Length >= MinPartialPhoneDigits)
        {
            var part = digits.TrimStart('0');
            return rows.Where(r => r.PhoneE164 != null && r.PhoneE164.Contains(part));
        }

        return rows.Where(_ => false);
    }

    internal static IQueryable<CustomerRow> Filter(IQueryable<CustomerRow> rows, CustomerListFilter filter, CustomerViewContext view)
    {
        var today = view.Today;
        switch (filter)
        {
            case CustomerListFilter.Regular:
                var regularSince = today.PlusDays(-CustomerSegments.RegularWindowDays);
                return rows.Where(r => r.VisitCount >= CustomerSegments.RegularMinVisits && r.LastVisitOn >= regularSince);
            case CustomerListFilter.New:
                var newSince = today.PlusDays(-CustomerSegments.NewCustomerDays);
                return rows.Where(r => r.VisitCount == 0 || r.FirstVisitOn >= newSince);
            case CustomerListFilter.NoShowRisk:
                var threshold = view.Settings.RiskyCustomerNoShowThreshold;
                return rows.Where(r => r.NoShowCount >= threshold);
            case CustomerListFilter.Lapsed60:
                var lapsedBefore = today.PlusDays(-CustomerSegments.LapsedDays);
                return rows.Where(r => r.LastVisitOn < lapsedBefore);
            default:
                return rows;
        }
    }

    // Sıra: (LastVisitOn yoksa sonda, LastVisitOn azalan, Id azalan); imleçten sonraki satırlar.
    private static IQueryable<CustomerRow> After(IQueryable<CustomerRow> rows, CustomerCursor cursor)
    {
        var id = cursor.Id;
        if (cursor.LastVisitOn is not { } day)
        {
            return rows.Where(r => r.LastVisitOn == null && r.Id.CompareTo(id) < 0);
        }

        return rows.Where(r => r.LastVisitOn == null || r.LastVisitOn < day || (r.LastVisitOn == day && r.Id.CompareTo(id) < 0));
    }

    private static string EscapeLike(string value) =>
        value.Replace(@"\", @"\\", StringComparison.Ordinal).Replace("%", @"\%", StringComparison.Ordinal).Replace("_", @"\_", StringComparison.Ordinal);
}
