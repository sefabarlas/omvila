using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features;

/// <summary>Tenancy panel uçlarının ortak yardımcıları: marka bağlamı, If-Match/ETag, telefon ve e-posta biçimi.</summary>
internal static partial class ManageEndpointSupport
{
    public static Guid BrandOf(ITenantContext tenant) => tenant.RequireBrandId();

    public static bool Matches<TEntity>(DbContext db, TEntity entity, uint current, uint expected)
        where TEntity : class => db.MatchesVersion(entity, current, expected);

    public static Ok<T> Versioned<T>(HttpContext http, uint version, T body) => VersionedResults.Versioned(http, version, body);

    /// <summary>Telefonu E.164'e çevirir; geçersizse alan hatası yazar ve değeri olduğu gibi döner.</summary>
    public static string? NormalizePhone(string? phone, string field, Dictionary<string, string[]> errors)
    {
        if (phone is null)
        {
            return null;
        }

        if (PhoneNumber.TryParse(phone, out var parsed))
        {
            return parsed.E164;
        }

        errors[field] = ["invalid_format"];
        return phone;
    }

    public static void CheckEmail(string? email, string field, Dictionary<string, string[]> errors)
    {
        if (email is null)
        {
            return;
        }

        if (email.Length > 254)
        {
            errors[field] = ["too_long"];
        }
        else if (!EmailPattern().IsMatch(email))
        {
            errors[field] = ["invalid_format"];
        }
    }

    public static string? MaxLength(string? value, string field, int max, Dictionary<string, string[]> errors)
    {
        if (value is not null && value.Length > max)
        {
            errors[field] = ["too_long"];
        }

        return value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex EmailPattern();
}
