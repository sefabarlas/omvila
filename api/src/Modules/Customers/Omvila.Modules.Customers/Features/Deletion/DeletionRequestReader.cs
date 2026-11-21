using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Deletion;

/// <summary>
/// AyarlarKvkk "Silme talepleri" (sözleşme manageListDeletionRequests): markanın talepleri, yeniden eskiye, opak imleç.
/// Bekleyen talepte kartın güncel adı, tamamlanan talepte yalnız maskeli ad döner.
/// </summary>
internal sealed class DeletionRequestReader(CustomersDbContext db)
{
    public async Task<Result<DataDeletionRequestPage>> ListAsync(Guid brandId, string? status, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        DeletionRequestStatus? statusFilter = null;
        if (status is not null)
        {
            if (Enum.TryParse<DeletionRequestStatus>(status, ignoreCase: false, out var s) && Enum.IsDefined(s))
            {
                statusFilter = s;
            }
            else
            {
                errors["status"] = ["invalid_format"];
            }
        }

        var take = limit ?? Customers.CustomerReader.DefaultLimit;
        if (take is < 1 or > Customers.CustomerReader.MaxLimit)
        {
            errors["limit"] = ["out_of_range"];
        }

        (Instant At, Guid Id)? after = null;
        if (cursor is not null)
        {
            if (TryDecode(cursor, out var decoded))
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

        var query = from r in db.DataDeletionRequests.AsNoTracking()
                    where r.BrandId == brandId
                    join b in db.BrandCustomers.AsNoTracking() on r.BrandCustomerId equals (Guid?)b.Id into cards
                    from b in cards.DefaultIfEmpty()
                    join c in db.Customers.AsNoTracking() on b.CustomerId equals c.Id into people
                    from c in people.DefaultIfEmpty()
                    select new { Request = r, LiveName = b == null ? null : b.DisplayName ?? c.FullName };
        if (statusFilter is { } f)
        {
            query = query.Where(x => x.Request.Status == f);
        }

        if (after is { } a)
        {
            query = query.Where(x => x.Request.RequestedUtc < a.At || (x.Request.RequestedUtc == a.At && x.Request.Id.CompareTo(a.Id) < 0));
        }

        var rows = await query.OrderByDescending(x => x.Request.RequestedUtc).ThenByDescending(x => x.Request.Id).Take(take + 1).ToListAsync(ct);
        var next = rows.Count > take ? Encode(rows[take - 1].Request.RequestedUtc, rows[take - 1].Request.Id) : null;
        return new DataDeletionRequestPage([.. rows.Take(take).Select(x => ToView(x.Request, x.LiveName))], next);
    }

    public static DataDeletionRequestView ToView(DataDeletionRequest r, string? liveName = null) => new(
        r.Id,
        r.Status == DeletionRequestStatus.Pending ? r.BrandCustomerId : null,
        r.Status == DeletionRequestStatus.Pending && liveName is not null ? liveName : r.CustomerNameMasked,
        r.RequestedVia.ToString(),
        r.RequestedUtc,
        r.DueOn,
        r.Status.ToString(),
        r.CompletedUtc);

    private static string Encode(Instant at, Guid id) =>
        WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(string.Create(CultureInfo.InvariantCulture, $"{at.ToUnixTimeTicks()}:{id}")));

    private static bool TryDecode(string cursor, out (Instant At, Guid Id) value)
    {
        value = default;
        try
        {
            var parts = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(cursor)).Split(':');
            if (parts.Length == 2 && long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks) && Guid.TryParse(parts[1], out var id))
            {
                value = (Instant.FromUnixTimeTicks(ticks), id);
                return true;
            }
        }
        catch (FormatException)
        {
            return false;
        }

        return false;
    }
}
