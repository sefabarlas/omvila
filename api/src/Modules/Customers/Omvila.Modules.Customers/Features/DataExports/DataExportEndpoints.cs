using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>
/// AyarlarKvkk "Tüm verileri dışa aktar" (sözleşme manageRequestDataExport; T-86 (7), T-121). İstek kuyruğa alınır (202);
/// arşivi ProcessDataExports işi hazırlar. Markada açık istek varsa yenisi açılmaz. Kullanıcı modelinde e-posta olmadığından
/// (alan-modeli.md § User) adres istekte zorunludur. Son isteğin durumu manageGetLatestDataExport ile okunur (rozet sayfa
/// yenilenince kaybolmasın; T-127 (2), T-132). İndirme yolu (publicDownloadDataExport) e-postadaki bağlantı içindir; panel ve
/// uygulamalar çağırmaz.
/// </summary>
internal static partial class DataExportEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/brand/data-exports", Audiences.Manage);
        group.MapPost(string.Empty, RequestAsync)
            .WithName("manageRequestDataExport")
            .RequirePermission(Permissions.Customer.Export)
            .RequireIdempotencyKey();
        group.MapGet("/latest", GetLatestAsync)
            .WithName("manageGetLatestDataExport")
            .RequirePermission(Permissions.Customer.Export);

        // Bağlantı e-postayla gider ve oturumsuz açılır: yetki jetonun imzası ve süresidir (DataExportLinks).
        app.MapApiGroup("/api/v1/public/data-exports")
            .MapGet("/{token}", DownloadAsync)
            .WithName("publicDownloadDataExport")
            .AllowAnonymous();
    }

    private static async Task<IResult> GetLatestAsync(ITenantContext tenant, CustomersDbContext db, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();

        // Arşiv (Content) yüklenmez: yalnız durum alanları okunur.
        var latest = await db.DataExports.AsNoTracking()
            .Where(e => e.BrandId == brandId)
            .OrderByDescending(e => e.RequestedUtc).ThenByDescending(e => e.Id)
            .Select(e => new { e.Id, e.Status, e.RequestedUtc, e.Email, e.ExpiresUtc })
            .FirstOrDefaultAsync(ct);
        return latest is null
            ? TypedResults.NoContent()
            : TypedResults.Ok(DataExportRequestResult.From(latest.Id, latest.Status, latest.RequestedUtc, latest.Email, latest.ExpiresUtc));
    }

    private static async Task<IResult> RequestAsync(
        DataExportRequestBody? body, HttpContext http, ITenantContext tenant, CustomersDbContext db, IAuditLog audit, IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var email = body?.Email?.Trim();
        if (string.IsNullOrEmpty(email))
        {
            return Error.Validation("email", "required").ToProblem();
        }

        if (email.Length > DataExport.EmailMaxLength || !EmailPattern().IsMatch(email))
        {
            return Error.Validation("email", email.Length > DataExport.EmailMaxLength ? "too_long" : "invalid_format").ToProblem();
        }

        var open = await db.DataExports
            .Where(e => e.BrandId == brandId && (e.Status == DataExportStatus.Queued || e.Status == DataExportStatus.Processing))
            .OrderBy(e => e.RequestedUtc)
            .FirstOrDefaultAsync(ct);
        var actor = AuditActor.From(http);
        var export = open ?? DataExport.Request(ids.New(), brandId, actor.UserId, email, clock.GetCurrentInstant());
        if (open is null)
        {
            db.DataExports.Add(export);
            await db.SaveChangesAsync(ct);
        }

        await audit.WriteAsync(
            actor.Entry(brandId, AuditAction.Export, nameof(DataExport), export.Id, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["scope"] = "Brand",
                ["reusedOpenRequest"] = open is not null,
            }),
            ct);
        return TypedResults.Accepted(
            (string?)null, DataExportRequestResult.From(export.Id, export.Status, export.RequestedUtc, export.Email, export.ExpiresUtc));
    }

    private static async Task<IResult> DownloadAsync(
        string token, IDbSession session, CustomersDbContext db, DataExportLinks links, IClock clock, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        if (links.Read(token, now) is not var (brandId, exportId))
        {
            return Error.NotFound().ToProblem();
        }

        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var export = await db.DataExports.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == exportId && e.BrandId == brandId && e.Content != null && e.ExpiresUtc > now, ct);
        return export is null
            ? Error.NotFound().ToProblem()
            : TypedResults.File(export.Content!, "application/zip", export.FileName);
    }

    /// <summary>"el***@ornek.com": yerel kısmın ilk iki harfi, alan adı açık.</summary>
    internal static string MaskEmail(string email)
    {
        var at = email.IndexOf('@', StringComparison.Ordinal);
        return at <= 0 ? "***" : $"{email[..Math.Min(2, at)]}***{email[at..]}";
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex EmailPattern();
}
