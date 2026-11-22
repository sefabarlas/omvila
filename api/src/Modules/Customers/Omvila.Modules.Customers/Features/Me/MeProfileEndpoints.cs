using System.Net.Mail;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>
/// Müşteri uygulaması profili (sözleşme meGetProfile, meUpdateProfile; "Profil", "Profil bilgilerim"). Global kişi kaydının
/// müşteriye görünen alanları; telefon giriş anahtarıdır, salt okunurdur. Kişi yalnız kendi kaydını görür (me jetonunun sub'ı).
/// </summary>
internal static class MeProfileEndpoints
{
    private const int FullNameMin = 2;
    private const int FullNameMax = 120;
    private const int EmailMax = 160;

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/profile", Audiences.Me);
        group.MapGet(string.Empty, GetAsync).WithName("meGetProfile").RequireAuthenticated(Audiences.Me);
        group.MapPatch(string.Empty, UpdateAsync).WithName("meUpdateProfile").RequireAuthenticated(Audiences.Me).RequireIfMatch();
    }

    private static async Task<IResult> GetAsync(HttpContext http, ITenantContext tenant, CustomersDbContext db, CancellationToken ct)
    {
        var customer = await db.Customers.AsNoTracking().SingleOrDefaultAsync(c => c.Id == MeScope.CustomerId(tenant) && c.AnonymizedUtc == null, ct);
        return customer is null ? Error.Unauthorized().ToProblem() : VersionedResults.Versioned(http, customer.Version, MeProfile.From(customer));
    }

    private static async Task<IResult> UpdateAsync(JsonElement body, HttpContext http, ITenantContext tenant, CustomersDbContext db, CancellationToken ct)
    {
        var customer = await db.Customers.SingleOrDefaultAsync(c => c.Id == MeScope.CustomerId(tenant) && c.AnonymizedUtc == null, ct);
        if (customer is null)
        {
            return Error.Unauthorized().ToProblem();
        }

        if (!db.MatchesVersion(customer, customer.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        if (body.ValueKind != JsonValueKind.Object)
        {
            return Error.Validation("body", "invalid_format").ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var fullName = ReadFullName(body, customer, errors);
        var email = ReadEmail(body, customer, errors);
        // Dil v1'de yalnız tr (sözleşme MeLanguage; T-101 (2)).
        if (body.TryGetProperty("language", out var language)
            && (language.ValueKind != JsonValueKind.String || !string.Equals(language.GetString(), Customer.DefaultLanguageCode, StringComparison.Ordinal)))
        {
            errors["language"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        customer.UpdateProfile(fullName, email);

        await db.SaveChangesAsync(ct);
        return VersionedResults.Versioned(http, customer.Version, MeProfile.From(customer));
    }

    private static string? ReadFullName(JsonElement body, Customer customer, Dictionary<string, string[]> errors)
    {
        if (!body.TryGetProperty("fullName", out var value))
        {
            return customer.HasName ? customer.FullName : null;
        }

        var text = value.ValueKind == JsonValueKind.String ? value.GetString()!.Trim() : null;
        if (text is null)
        {
            errors["fullName"] = ["invalid_format"];
        }
        else if (text.Length < FullNameMin)
        {
            errors["fullName"] = ["too_short"];
        }
        else if (text.Length > FullNameMax)
        {
            errors["fullName"] = ["too_long"];
        }

        return text;
    }

    // Gönderilmeyen e-posta değişmez; açıkça null gönderilirse silinir (sözleşme meUpdateProfile).
    private static string? ReadEmail(JsonElement body, Customer customer, Dictionary<string, string[]> errors)
    {
        if (!body.TryGetProperty("email", out var value))
        {
            return customer.Email;
        }

        if (value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var text = value.ValueKind == JsonValueKind.String ? value.GetString()!.Trim() : null;
        if (text is null || !IsEmail(text))
        {
            errors["email"] = ["invalid_format"];
        }
        else if (text.Length > EmailMax)
        {
            errors["email"] = ["too_long"];
        }

        return text;
    }

    private static bool IsEmail(string text) =>
        text.Length > 0 && !text.Contains(' ', StringComparison.Ordinal) && MailAddress.TryCreate(text, out var address)
        && string.Equals(address.Address, text, StringComparison.Ordinal) && address.Host.Contains('.', StringComparison.Ordinal);
}
