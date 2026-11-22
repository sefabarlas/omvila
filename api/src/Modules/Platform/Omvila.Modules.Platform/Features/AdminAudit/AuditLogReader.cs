using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminAudit;

/// <summary>
/// Denetim kaydı okuma ve CSV dışa aktarma (sözleşme adminListAuditLog, adminExportAuditLog; PDenetim). Kategori eşlemesi:
/// View = View; ApproveReject = Approve, Match ve PaymentNotice/BrandSmsSender güncellemeleri; CreditsAndPlan = CreditTransaction ve
/// Subscription kayıtları; Kvkk = Anonymize ve Export. fromUtc verilmezse son 24 saat. Yeniden eskiye (kayıt numarası). Ad ve
/// telefon <see cref="AuditMask"/> ile maskelidir; müşteri işlemlerinde ad dönmez (T-124).
/// </summary>
internal sealed class AuditLogReader(PlatformDbContext db, IPlatformBrandDirectory brands, IUserNameDirectory users, IClock clock)
{
    public const int DefaultLimit = 50;
    private const int ExportMaxDays = 92;
    private static readonly Duration DefaultWindow = Duration.FromHours(24);

    // CSV'de Türkçe harfler kaçırılmadan yazılır (hücre zaten tırnaklı ve kaçırılmış).
    private static readonly JsonSerializerOptions CsvJson = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
    private static readonly string[] ApproveRejectEntities = [nameof(PaymentNotice), "BrandSmsSender"];
    private static readonly string[] CreditsAndPlanEntities = ["CreditTransaction", "Subscription"];

    public async Task<Result<AuditLogPage>> ListAsync(AuditQuery query, int? limit, string? cursor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(query);
        var errors = Validate(query, out var filter);
        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        long? before = null;
        if (cursor is not null)
        {
            if (long.TryParse(cursor, NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id > 0)
            {
                before = id;
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

        var rows = await FilteredAsync(filter!, ct);
        if (before is { } b)
        {
            rows = rows.Where(l => l.Id < b);
        }

        var take = limit ?? DefaultLimit;
        var page = await rows.OrderByDescending(l => l.Id).Take(take + 1).ToListAsync(ct);
        var items = await MapAsync([.. page.Take(take)], ct);
        return new AuditLogPage(items, page.Count > take ? items[^1].Id.ToString(CultureInfo.InvariantCulture) : null);
    }

    /// <summary>CSV (UTF-8 BOM'lu, noktalı virgül ayraçlı); aralık en fazla 92 gün.</summary>
    public async Task<Result<(byte[] Content, int Rows, AuditFilter Filter)>> ExportAsync(AuditQuery query, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(query);
        var errors = Validate(query, out var filter);
        if (filter is not null && (filter.ToUtc ?? clock.GetCurrentInstant()) - filter.FromUtc > Duration.FromDays(ExportMaxDays))
        {
            errors["toUtc"] = ["out_of_range"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var rows = await (await FilteredAsync(filter!, ct)).OrderByDescending(l => l.Id).ToListAsync(ct);
        var items = await MapAsync(rows, ct);
        var zone = DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var csv = new StringBuilder();
        csv.Append("Zaman;Kim;Rol;İşlem;Varlık;Marka;Ayrıntı\r\n");
        foreach (var item in items)
        {
            Line(csv, [
                item.OccurredUtc.InZone(zone).ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
                string.Equals(item.Actor.Type, nameof(AuditActorType.System), StringComparison.Ordinal) ? "Sistem" : item.Actor.Name ?? string.Empty,
                item.Actor.PlatformRole ?? string.Empty,
                item.Action.ToString(),
                item.EntityType,
                item.Brand?.Name ?? string.Empty,
                item.Changes.ToJsonString(CsvJson)]);
        }

        return ([.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(csv.ToString())], items.Count, filter!);
    }

    private Dictionary<string, string[]> Validate(AuditQuery query, out AuditFilter? filter)
    {
        filter = null;
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var category = query.Category ?? "All";
        if (category is not ("All" or "View" or "ApproveReject" or "CreditsAndPlan" or "Kvkk"))
        {
            errors["category"] = ["invalid_format"];
        }

        var search = query.Q?.Trim();
        if (search is { Length: < 2 or > 100 })
        {
            errors["q"] = ["out_of_range"];
        }

        Guid? brandId = null;
        if (query.BrandId is not null)
        {
            if (Guid.TryParse(query.BrandId, out var parsed))
            {
                brandId = parsed;
            }
            else
            {
                errors["brandId"] = ["invalid_format"];
            }
        }

        var from = ParseInstant(query.FromUtc, "fromUtc", errors) ?? clock.GetCurrentInstant() - DefaultWindow;
        var to = ParseInstant(query.ToUtc, "toUtc", errors);
        if (to is { } t && t <= from)
        {
            errors["toUtc"] = ["out_of_range"];
        }

        if (errors.Count == 0)
        {
            filter = new AuditFilter(category, string.IsNullOrEmpty(search) ? null : search, brandId, from, to);
        }

        return errors;
    }

    private async Task<IQueryable<AuditLogEntry>> FilteredAsync(AuditFilter filter, CancellationToken ct)
    {
        var rows = db.AuditLogs.AsNoTracking().Where(l => l.OccurredUtc >= filter.FromUtc);
        if (filter.ToUtc is { } to)
        {
            rows = rows.Where(l => l.OccurredUtc < to);
        }

        if (filter.BrandId is { } brandId)
        {
            rows = rows.Where(l => l.BrandId == brandId);
        }

        rows = filter.Category switch
        {
            "View" => rows.Where(l => l.Action == AuditAction.View),
            "ApproveReject" => rows.Where(l => l.Action == AuditAction.Approve || l.Action == AuditAction.Match
                || (l.Action == AuditAction.Update && ApproveRejectEntities.Contains(l.EntityType))),
            "CreditsAndPlan" => rows.Where(l => CreditsAndPlanEntities.Contains(l.EntityType)),
            "Kvkk" => rows.Where(l => l.Action == AuditAction.Anonymize || l.Action == AuditAction.Export),
            _ => rows,
        };

        if (filter.Query is { } q)
        {
            var brandIds = (await brands.ListAsync(ct))
                .Where(b => CultureInfo.GetCultureInfo("tr-TR").CompareInfo.IndexOf(b.Name, q, CompareOptions.IgnoreCase) >= 0)
                .Select(b => (Guid?)b.Id)
                .ToList();
            var actorIds = (await users.FindByNameAsync(q, ct)).Select(id => (Guid?)id).ToList();
            rows = rows.Where(l => brandIds.Contains(l.BrandId) || actorIds.Contains(l.ActorId));
        }

        return rows;
    }

    private async Task<List<AuditLogItem>> MapAsync(IReadOnlyList<AuditLogEntry> rows, CancellationToken ct)
    {
        var actorIds = rows.Where(r => r.ActorId is not null && r.ActorType is AuditActorType.User or AuditActorType.PlatformUser)
            .Select(r => r.ActorId!.Value).Distinct().ToList();
        var names = actorIds.Count == 0 ? new Dictionary<Guid, UserNameInfo>() : (Dictionary<Guid, UserNameInfo>)(await users.GetAsync(actorIds, ct)).ToDictionary();
        var brandNames = rows.Any(r => r.BrandId is not null) ? (await brands.ListAsync(ct)).ToDictionary(b => b.Id, b => b.Name) : [];
        return [.. rows.Select(r =>
        {
            var info = r.ActorId is { } id && r.ActorType != AuditActorType.Customer ? names.GetValueOrDefault(id) : null;
            var actor = new AuditActor(
                r.ActorType.ToString(),
                r.ActorType == AuditActorType.System ? null : r.ActorId,
                info?.FullName is { Length: > 0 } fullName ? fullName : null,
                r.ActorType == AuditActorType.PlatformUser ? info?.PlatformRole : null);
            var brand = r.BrandId is { } b ? new AuditBrandRef(b, brandNames.GetValueOrDefault(b) ?? string.Empty) : null;
            return new AuditLogItem(r.Id, r.OccurredUtc, actor, r.Action, r.EntityType, r.EntityId?.ToString(), brand, AuditMask.Changes(r.Changes));
        })];
    }

    private static Instant? ParseInstant(string? text, string field, Dictionary<string, string[]> errors)
    {
        if (text is null)
        {
            return null;
        }

        var parsed = InstantPattern.ExtendedIso.Parse(text);
        if (parsed.Success)
        {
            return parsed.Value;
        }

        errors[field] = ["invalid_format"];
        return null;
    }

    // Değerler tırnaklanır; formül olarak çalışabilecek baş karakterlere tek tırnak eklenir (CSV enjeksiyonu).
    private static void Line(StringBuilder csv, string[] cells)
    {
        for (var i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            if (cell.Length > 0 && cell[0] is '=' or '+' or '-' or '@' or '\t' or '\r')
            {
                cell = "'" + cell;
            }

            csv.Append('"').Append(cell.Replace("\"", "\"\"", StringComparison.Ordinal)).Append('"').Append(i == cells.Length - 1 ? "\r\n" : ";");
        }
    }

    /// <summary>Sorgu parametreleri (sözleşme AuditCategory, AuditQuery, AuditBrandId, AuditFromUtc, AuditToUtc).</summary>
    public sealed record AuditQuery(string? Category, string? Q, string? BrandId, string? FromUtc, string? ToUtc);

    /// <summary>Doğrulanmış süzgeç.</summary>
    public sealed record AuditFilter(string Category, string? Query, Guid? BrandId, Instant FromUtc, Instant? ToUtc);

    /// <summary>Sözleşme AuditLogPage.</summary>
    public sealed record AuditLogPage(IReadOnlyList<AuditLogItem> Items, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);

    /// <summary>Sözleşme AuditLogEntry.</summary>
    public sealed record AuditLogItem(
        long Id,
        Instant OccurredUtc,
        AuditActor Actor,
        AuditAction Action,
        string EntityType,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? EntityId,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] AuditBrandRef? Brand,
        JsonObject Changes);

    /// <summary>"Kim".</summary>
    public sealed record AuditActor(
        string Type,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? Id,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Name,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PlatformRole);

    /// <summary>Sözleşme AdminBrandRef.</summary>
    public sealed record AuditBrandRef(Guid Id, string Name);
}
