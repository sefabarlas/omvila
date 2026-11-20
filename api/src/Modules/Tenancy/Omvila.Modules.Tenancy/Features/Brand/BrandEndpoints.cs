using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>
/// İşletme, randevu kuralları ve rezervasyon sayfası uçları (sözleşme manage-brand; AyarlarIsletme, AyarlarRezervasyon).
/// Okuma her rolde açıktır; değiştirmek settings.manage ister. Düzenlemeler If-Match ile korunur (sürüm = xmin).
/// </summary>
internal static partial class BrandEndpoints
{
    private static readonly HashSet<string> IndustryCodes =
        new(["barber", "hair_salon", "beauty", "spa", "nail", "dietitian", "other"], StringComparer.Ordinal);

    public static void Map(IEndpointRouteBuilder app)
    {
        var brand = app.MapApiGroup("/api/v1/manage/brand", Audiences.Manage);

        brand.MapGet(string.Empty, GetBrandAsync).WithName("manageGetBrand").RequireAuthenticated(Audiences.Manage);
        brand.MapPatch(string.Empty, UpdateBrandAsync).WithName("manageUpdateBrand")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();

        brand.MapGet("/settings", GetSettingsAsync).WithName("manageGetBrandSettings").RequireAuthenticated(Audiences.Manage);
        brand.MapPut("/settings", UpdateSettingsAsync).WithName("manageUpdateBrandSettings")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();

        brand.MapGet("/booking-page", GetBookingPageAsync).WithName("manageGetBookingPageSettings").RequireAuthenticated(Audiences.Manage);
        brand.MapPut("/booking-page", UpdateBookingPageAsync).WithName("manageUpdateBookingPageSettings")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();
    }

    private static async Task<IResult> GetBrandAsync(HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var brand = await db.Brands.AsNoTracking().SingleAsync(b => b.Id == BrandOf(tenant), ct);
        return Versioned(http, brand.Version, BrandResponse.From(brand));
    }

    private static async Task<IResult> UpdateBrandAsync(
        JsonElement body, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var brand = await db.Brands.SingleAsync(b => b.Id == BrandOf(tenant), ct);
        if (!Matches(db, brand, brand.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var input = new JsonBody(body);
        if (!input.IsObject)
        {
            return Error.Validation("body", "invalid_format").ToProblem();
        }

        var profile = ReadProfile(input, brand);
        if (input.Errors.Count > 0)
        {
            return Error.Validation(input.Errors).ToProblem();
        }

        brand.UpdateProfile(profile);
        await db.SaveChangesAsync(ct);
        return Versioned(http, brand.Version, BrandResponse.From(brand));
    }

    private static async Task<IResult> GetSettingsAsync(HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var settings = await db.BrandSettings.AsNoTracking().SingleAsync(s => s.BrandId == BrandOf(tenant), ct);
        return Versioned(http, settings.Version, BrandSettingsResponse.From(settings, DepositAvailable));
    }

    private static async Task<IResult> UpdateSettingsAsync(
        BrandSettingsRequest body, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var settings = await db.BrandSettings.SingleAsync(s => s.BrandId == BrandOf(tenant), ct);
        if (!Matches(db, settings, settings.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var values = body.ToValues(out var missing);
        var errors = values is null ? missing : BrandSettingsRules.Validate(values);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        // S-22: IBAN (Dilim 4) yokken kapora açılamaz; var olan değer (kayıt varsayılanı dahil) korunabilir.
        if (!DepositAvailable && !string.Equals(values!.DepositPolicy, settings.DepositPolicy, StringComparison.Ordinal)
            && !string.Equals(values.DepositPolicy, "None", StringComparison.Ordinal))
        {
            return Error.Conflict(ErrorCodes.DepositUnavailableNoIban).ToProblem();
        }

        settings.Apply(values!);
        db.Entry(settings).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return Versioned(http, settings.Version, BrandSettingsResponse.From(settings, DepositAvailable));
    }

    private static async Task<IResult> GetBookingPageAsync(
        HttpContext http, ITenantContext tenant, TenancyDbContext db, IOptions<TenancyOptions> options, CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var brand = await db.Brands.AsNoTracking().SingleAsync(b => b.Id == brandId, ct);
        var page = await db.BookingPageSettings.AsNoTracking().SingleAsync(p => p.BrandId == brandId, ct);
        return Versioned(http, page.Version, BookingPageResponse.From(brand.Slug, page, options.Value.BookingPageBaseUrl!));
    }

    private static async Task<IResult> UpdateBookingPageAsync(
        BookingPageRequest body,
        HttpContext http,
        ITenantContext tenant,
        TenancyDbContext db,
        SlugRegistry slugs,
        IIdGenerator ids,
        IClock clock,
        IOptions<TenancyOptions> options,
        CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var page = await db.BookingPageSettings.SingleAsync(p => p.BrandId == brandId, ct);
        if (!Matches(db, page, page.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var errors = body.Validate();
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var slug = Slug.Create(body.Slug);
        if (!slug.IsSuccess)
        {
            return slug.Error.ToProblem();
        }

        var brand = await db.Brands.SingleAsync(b => b.Id == brandId, ct);
        if (!string.Equals(brand.Slug, slug.Value.Value, StringComparison.Ordinal))
        {
            await slugs.LockAsync(ct);
            var now = clock.GetCurrentInstant();
            var available = SlugAvailability.Check(slug.Value, brandId, await slugs.UsesAsync(slug.Value.Value, ct), now);
            if (!available.IsSuccess)
            {
                return available.Error.ToProblem();
            }

            if (brand.ChangeSlug(slug.Value, ids.New(), now) is { } history)
            {
                db.SlugHistory.Add(history);
            }
        }

        page.Update(body.CoverColor!, body.ShowReviews!.Value, body.ShowPrices!.Value, body.PreAppointmentNote);
        db.Entry(page).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return Versioned(http, page.Version, BookingPageResponse.From(brand.Slug, page, options.Value.BookingPageBaseUrl!));
    }

    // Kapora için salon IBAN'ı (BrandBankAccount) Dilim 4'te gelir; o zamana kadar kapora kullanılamaz (S-22).
    private static bool DepositAvailable => false;

    private static BrandProfile ReadProfile(JsonBody input, Domain.Brand brand)
    {
        var name = input.String("name", brand.Name);
        if (input.Has("name") && (name is null || name.Length < 2))
        {
            input.Errors["name"] = [name is null ? "required" : "too_short"];
        }

        MaxLength(input, "name", name, 160);
        var industry = input.String("industryCode", brand.IndustryCode);
        if (industry is null || !IndustryCodes.Contains(industry))
        {
            input.Errors["industryCode"] = ["invalid_format"];
        }

        var description = MaxLength(input, "shortDescription", input.String("shortDescription", brand.ShortDescription), 160);
        var phone = input.String("phone", brand.Phone);
        if (input.Has("phone") && phone is not null)
        {
            if (PhoneNumber.TryParse(phone, out var parsed))
            {
                phone = parsed.E164;
            }
            else
            {
                input.Errors["phone"] = ["invalid_format"];
            }
        }

        var email = MaxLength(input, "email", input.String("email", brand.Email), 254);
        if (input.Has("email") && email is not null && !EmailPattern().IsMatch(email))
        {
            input.Errors["email"] = ["invalid_format"];
        }

        return new BrandProfile(
            name ?? brand.Name,
            industry ?? brand.IndustryCode,
            description,
            phone,
            email,
            MaxLength(input, "instagram", input.String("instagram", brand.Instagram), 60),
            MaxLength(input, "website", input.String("website", brand.Website), 200),
            MaxLength(input, "legalName", input.String("legalName", brand.LegalName), 200),
            MaxLength(input, "taxOffice", input.String("taxOffice", brand.TaxOffice), 80),
            MaxLength(input, "taxNumber", input.String("taxNumber", brand.TaxNumber), 20));
    }

    private static string? MaxLength(JsonBody input, string field, string? value, int max)
    {
        if (value is not null && value.Length > max)
        {
            input.Errors[field] = ["too_long"];
        }

        return value;
    }

    /// <summary>If-Match sürümü güncel mi; değilse 412. Ayrıca kaydetmede xmin denetimi için özgün değer atanır.</summary>
    private static bool Matches<TEntity>(DbContext db, TEntity entity, uint current, uint expected)
        where TEntity : class
    {
        db.Entry(entity).Property<uint>("Version").OriginalValue = expected;
        return current == expected;
    }

    private static Microsoft.AspNetCore.Http.HttpResults.Ok<T> Versioned<T>(HttpContext http, uint version, T body)
    {
        http.Response.Headers.ETag = ETagVersion.Format(version);
        return TypedResults.Ok(body);
    }

    private static Guid BrandOf(ITenantContext tenant) =>
        tenant.Scope.BrandId ?? throw new DomainException("Manage isteğinde marka bağlamı yok.");

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex EmailPattern();
}
