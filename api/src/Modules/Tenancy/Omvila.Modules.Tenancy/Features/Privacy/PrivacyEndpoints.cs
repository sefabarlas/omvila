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
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Brand;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.Privacy;

/// <summary>
/// Ayarlar → KVKK ve veri (sözleşme manageGetPrivacySettings / manageUpdatePrivacySettings; T-86 (7)). Değerler marka ayarları
/// satırında; sürüm o satırın xmin'idir. Değişiklik denetime yazılır (metin içeriği değil, yalnız seçim ve uzunluk).
/// </summary>
internal static class PrivacyEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/brand/privacy", Audiences.Manage);
        group.MapGet(string.Empty, GetAsync).WithName("manageGetPrivacySettings").RequirePermission(Permissions.Settings.Manage);
        group.MapPut(string.Empty, UpdateAsync).WithName("manageUpdatePrivacySettings")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();
    }

    private static async Task<IResult> GetAsync(
        HttpContext http, ITenantContext tenant, TenancyDbContext db, IOptions<PrivacyNoticeOptions> notice, CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var settings = await db.BrandSettings.AsNoTracking().SingleAsync(s => s.BrandId == brandId, ct);
        var brand = await db.Brands.AsNoTracking().SingleAsync(b => b.Id == brandId, ct);
        return Versioned(http, settings.Version, Response(settings, brand, notice.Value));
    }

    private static async Task<IResult> UpdateAsync(
        JsonElement body,
        HttpContext http,
        ITenantContext tenant,
        TenancyDbContext db,
        IOptions<PrivacyNoticeOptions> notice,
        IAuditLog audit,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var settings = await db.BrandSettings.SingleAsync(s => s.BrandId == brandId, ct);
        if (!Matches(db, settings, settings.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var input = new JsonBody(body);
        var mode = input.String("noticeMode", null);
        var textProvided = input.Has("customNoticeText");
        // Metin olduğu gibi saklanır (yalnız baştaki ve sondaki boşluk atılır); JsonBody boş metni null sayar.
        var text = input.String("customNoticeText", settings.CustomPrivacyNoticeText);
        var years = input.Int("transactionRetentionYears", null);
        var errors = PrivacySettingsRules.Validate(mode, textProvided, text, settings.CustomPrivacyNoticeText, years);
        foreach (var (field, codes) in input.Errors)
        {
            errors[field] = codes;
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var previous = (settings.PrivacyNoticeMode, settings.TransactionRetentionYears);
        settings.ApplyPrivacy(mode!, textProvided, text, years!.Value, clock.GetCurrentInstant());
        db.Entry(settings).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);

        var userId = Guid.TryParse(http.User.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : (Guid?)null;
        await audit.WriteAsync(
            new AuditEntry(
                brandId,
                AuditActorType.User,
                userId,
                AuditAction.Update,
                "PrivacySettings",
                brandId,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["previousNoticeMode"] = previous.PrivacyNoticeMode,
                    ["noticeMode"] = settings.PrivacyNoticeMode,
                    ["customNoticeLength"] = settings.CustomPrivacyNoticeText?.Length,
                    ["previousTransactionRetentionYears"] = previous.TransactionRetentionYears,
                    ["transactionRetentionYears"] = settings.TransactionRetentionYears,
                },
                http.Connection.RemoteIpAddress?.ToString(),
                http.Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null),
            ct);

        var brand = await db.Brands.AsNoTracking().SingleAsync(b => b.Id == brandId, ct);
        return Versioned(http, settings.Version, Response(settings, brand, notice.Value));
    }

    private static PrivacySettingsResponse Response(BrandSettings s, Domain.Brand brand, PrivacyNoticeOptions notice) => new(
        s.PrivacyNoticeMode,
        new StandardNoticeView(
            notice.PrivacyNoticeVersion, notice.UpdatedOn!.Value, StandardPrivacyNotice.Render(brand.Name, brand.LegalName, brand.Email, brand.Phone)),
        s.CustomPrivacyNoticeText,
        s.CustomPrivacyNoticeUpdatedUtc,
        s.TransactionRetentionYears,
        PrivacySettingsRules.AllowedRetentionYears,
        ETagVersion.Format(s.Version));
}
