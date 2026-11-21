using System.Text.Json;
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
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>
/// İşletme, randevu kuralları ve rezervasyon sayfası uçları (sözleşme manage-brand; AyarlarIsletme, AyarlarRezervasyon).
/// Okuma her rolde açıktır; değiştirmek settings.manage ister. Düzenlemeler If-Match ile korunur (sürüm = xmin).
/// </summary>
internal static class BrandEndpoints
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
        return Versioned(http, settings.Version, BrandSettingsResponse.From(settings, await DepositAvailableAsync(db, settings.BrandId, ct)));
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

        // S-22: aktif kapora hesabı (IBAN) yokken kapora açılamaz; var olan değer (kayıt varsayılanı dahil) korunabilir.
        var depositAvailable = await DepositAvailableAsync(db, settings.BrandId, ct);
        if (!depositAvailable && !string.Equals(values!.DepositPolicy, settings.DepositPolicy, StringComparison.Ordinal)
            && !string.Equals(values.DepositPolicy, "None", StringComparison.Ordinal))
        {
            return Error.Conflict(ErrorCodes.DepositUnavailableNoIban).ToProblem();
        }

        settings.Apply(values!);
        db.Entry(settings).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return Versioned(http, settings.Version, BrandSettingsResponse.From(settings, depositAvailable));
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

    // Kural: alan-modeli.md § BrandBankAccount — aktif hesap yoksa kapora konamaz (sözleşme BrandSettings.depositAvailable).
    private static Task<bool> DepositAvailableAsync(TenancyDbContext db, Guid brandId, CancellationToken ct) =>
        db.BankAccounts.AsNoTracking().AnyAsync(a => a.BrandId == brandId && a.IsActive, ct);

    private static BrandProfile ReadProfile(JsonBody input, Domain.Brand brand)
    {
        var name = input.String("name", brand.Name);
        if (input.Has("name") && (name is null || name.Length < 2))
        {
            input.Errors["name"] = [name is null ? "required" : "too_short"];
        }

        MaxLength(name, "name", 160, input.Errors);
        var industry = input.String("industryCode", brand.IndustryCode);
        if (industry is null || !IndustryCodes.Contains(industry))
        {
            input.Errors["industryCode"] = ["invalid_format"];
        }

        var description = MaxLength(input.String("shortDescription", brand.ShortDescription), "shortDescription", 160, input.Errors);
        var phone = input.Has("phone") ? NormalizePhone(input.String("phone", null), "phone", input.Errors) : brand.Phone;
        var email = input.String("email", brand.Email);
        if (input.Has("email"))
        {
            CheckEmail(email, "email", input.Errors);
        }

        return new BrandProfile(
            name ?? brand.Name,
            industry ?? brand.IndustryCode,
            description,
            phone,
            email,
            MaxLength(input.String("instagram", brand.Instagram), "instagram", 60, input.Errors),
            MaxLength(input.String("website", brand.Website), "website", 200, input.Errors),
            MaxLength(input.String("legalName", brand.LegalName), "legalName", 200, input.Errors),
            MaxLength(input.String("taxOffice", brand.TaxOffice), "taxOffice", 80, input.Errors),
            MaxLength(input.String("taxNumber", brand.TaxNumber), "taxNumber", 20, input.Errors));
    }
}
