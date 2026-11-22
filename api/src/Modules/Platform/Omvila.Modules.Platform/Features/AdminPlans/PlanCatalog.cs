using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminPlans.AdminPlanViews;

namespace Omvila.Modules.Platform.Features.AdminPlans;

/// <summary>
/// Planlar ve kontör paketleri (sözleşme adminListPlans, adminUpdatePlans, adminListCreditPackages, adminUpdateCreditPackages; PPlanlar;
/// T-116 (14), (15)). S-14 fiyat ve sınırları buradan girilir; kodda değer yoktur. Plan ve paket eklenmez, silinmez; code ile eşleşir.
/// Katalog sürümü satır sürümlerinin toplamıdır (If-Match). Ortak kurallar yapılandırmadan salt okunurdur.
/// </summary>
internal sealed class PlanCatalog(PlatformDbContext db, IOptions<PlatformOptions> options)
{
    public async Task<(AdminPlanCatalog Catalog, uint Version)> GetPlansAsync(CancellationToken ct)
    {
        var plans = await db.Plans.AsNoTracking().OrderBy(p => p.MonthlyPriceMinor == null).ThenBy(p => p.MonthlyPriceMinor).ThenBy(p => p.Code).ToListAsync(ct);
        var counts = await db.Subscriptions.AsNoTracking()
            .Where(s => s.Status != SubscriptionStatus.Cancelled)
            .GroupBy(s => s.PlanId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count, ct);
        var version = Sum(plans.Select(p => p.Version));
        var settings = options.Value;
        return (new AdminPlanCatalog(
            [.. plans.Select(p => new AdminPlan(
                p.Code, p.Name, p.Description, Price(p.MonthlyPriceMinor), Price(p.YearlyPriceMinor), p.IncludedBranchCount, Price(p.ExtraBranchPriceMinor),
                p.StaffLimit, p.IncludedSmsCredits, p.Features, p.IsPublic, p.IsActive, counts.GetValueOrDefault(p.Id)))],
            new PlanRules(settings.TrialDays, settings.PastDueGraceDays, settings.TrialCredits),
            ETagVersion.Format(version)), version);
    }

    public async Task<(AdminCreditPackageCatalog Catalog, uint Version)> GetPackagesAsync(CancellationToken ct)
    {
        var packages = await db.CreditPackages.AsNoTracking().OrderBy(p => p.Credits).ThenBy(p => p.Code).ToListAsync(ct);
        var version = Sum(packages.Select(p => p.Version));
        return (new AdminCreditPackageCatalog(
            [.. packages.Select(p => new AdminCreditPackage(p.Code, p.Name, p.Credits, Price(p.PriceMinor), p.IsActive))], ETagVersion.Format(version)), version);
    }

    /// <summary>Planları günceller; değişen her plan için denetim girdisi döner (çağıran yazar).</summary>
    public async Task<Result<IReadOnlyList<(Guid Id, Dictionary<string, object?> Changes)>>> UpdatePlansAsync(
        AdminPlanCatalogUpdate body, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        var plans = await db.Plans.ToListAsync(ct);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var updates = new List<(Plan Plan, PlanValues Values)>();
        if (body.Plans is not { Count: > 0 } items)
        {
            return Error.Validation("plans", "required");
        }

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var plan = plans.SingleOrDefault(p => string.Equals(p.Code, item.Code, StringComparison.Ordinal));
            if (plan is null)
            {
                errors[$"plans[{i}].code"] = [item.Code is null ? "required" : "invalid_format"];
                continue;
            }

            if (PlanValuesFrom(item, $"plans[{i}]", errors) is { } values)
            {
                updates.Add((plan, values));
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        if (Sum(plans.Select(p => p.Version)) != expectedVersion)
        {
            return Error.PreconditionFailed();
        }

        var audits = new List<(Guid, Dictionary<string, object?>)>();
        foreach (var (plan, values) in updates)
        {
            var before = plan.Values();
            if (before == values)
            {
                continue;
            }

            plan.Update(values);
            audits.Add((plan.Id, PlanChanges(plan.Code, before, values)));
        }

        await db.SaveChangesAsync(ct);
        return audits;
    }

    /// <summary>Kontör paketlerini günceller; değişen her paket için denetim girdisi döner.</summary>
    public async Task<Result<IReadOnlyList<(Guid Id, Dictionary<string, object?> Changes)>>> UpdatePackagesAsync(
        AdminCreditPackageCatalogUpdate body, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        if (body.Packages is not { Count: > 0 } items)
        {
            return Error.Validation("packages", "required");
        }

        var packages = await db.CreditPackages.ToListAsync(ct);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var updates = new List<(CreditPackage Package, string Name, int Credits, long? Price, bool IsActive)>();
        for (var i = 0; i < items.Count; i++)
        {
            if (PackageFrom(items[i], $"packages[{i}]", packages, errors) is { } update)
            {
                updates.Add(update);
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        if (Sum(packages.Select(p => p.Version)) != expectedVersion)
        {
            return Error.PreconditionFailed();
        }

        var audits = new List<(Guid, Dictionary<string, object?>)>();
        foreach (var (package, name, credits, price, active) in updates)
        {
            if (string.Equals(package.Name, name, StringComparison.Ordinal) && package.Credits == credits && package.PriceMinor == price && package.IsActive == active)
            {
                continue;
            }

            var changes = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["code"] = package.Code,
                ["credits"] = new[] { package.Credits, credits },
                ["priceMinor"] = new[] { package.PriceMinor, price },
                ["isActive"] = new[] { package.IsActive, active },
            };
            package.Update(name, credits, price, active);
            audits.Add((package.Id, changes));
        }

        await db.SaveChangesAsync(ct);
        return audits;
    }

    private static (CreditPackage Package, string Name, int Credits, long? Price, bool IsActive)? PackageFrom(
        AdminCreditPackageUpdate item, string prefix, List<CreditPackage> packages, Dictionary<string, string[]> errors)
    {
        var package = packages.SingleOrDefault(p => string.Equals(p.Code, item.Code, StringComparison.Ordinal));
        if (package is null)
        {
            errors[$"{prefix}.code"] = [item.Code is null ? "required" : "invalid_format"];
            return null;
        }

        var count = errors.Count;
        var name = Text(item.Name, $"{prefix}.name", 2, Plan.NameMaxLength, errors);
        if (item.Credits is not { } credits || credits < 1)
        {
            errors[$"{prefix}.credits"] = [item.Credits is null ? "required" : "out_of_range"];
        }

        var price = MoneyMinor(item.Price, $"{prefix}.price", errors);
        if (item.IsActive is null)
        {
            errors[$"{prefix}.isActive"] = ["required"];
        }

        return errors.Count > count ? null : (package, name!, item.Credits!.Value, price, item.IsActive!.Value);
    }

    private static PlanValues? PlanValuesFrom(AdminPlanUpdate item, string prefix, Dictionary<string, string[]> errors)
    {
        var count = errors.Count;
        var name = Text(item.Name, $"{prefix}.name", 2, Plan.NameMaxLength, errors);
        var description = item.Description?.Trim();
        if (description is { Length: > Plan.DescriptionMaxLength })
        {
            errors[$"{prefix}.description"] = ["out_of_range"];
        }

        var monthly = MoneyMinor(item.MonthlyPrice, $"{prefix}.monthlyPrice", errors);
        var yearly = MoneyMinor(item.YearlyPrice, $"{prefix}.yearlyPrice", errors);
        var extra = MoneyMinor(item.ExtraBranchPrice, $"{prefix}.extraBranchPrice", errors);
        Minimum(item.IncludedBranchCount, 1, $"{prefix}.includedBranchCount", errors);
        Minimum(item.StaffLimit, 1, $"{prefix}.staffLimit", errors);
        Minimum(item.IncludedSmsCredits, 0, $"{prefix}.includedSmsCredits", errors);
        var f = item.Features;
        if (f?.WalkInQueue is null || f.Waitlist is null || f.Packages is null || f.CrossBranchReports is null || f.PrioritySupport is null)
        {
            errors[$"{prefix}.features"] = ["required"];
        }

        if (item.IsPublic is null)
        {
            errors[$"{prefix}.isPublic"] = ["required"];
        }

        if (item.IsActive is null)
        {
            errors[$"{prefix}.isActive"] = ["required"];
        }

        return errors.Count > count
            ? null
            : new PlanValues(
                name!, string.IsNullOrEmpty(description) ? null : description, monthly, yearly, item.IncludedBranchCount, extra, item.StaffLimit,
                item.IncludedSmsCredits,
                new PlanFeatures(f!.WalkInQueue!.Value, f.Waitlist!.Value, f.Packages!.Value, f.CrossBranchReports!.Value, f.PrioritySupport!.Value),
                item.IsPublic!.Value, item.IsActive!.Value);
    }

    private static Dictionary<string, object?> PlanChanges(string code, PlanValues before, PlanValues after)
    {
        var changes = new Dictionary<string, object?>(StringComparer.Ordinal) { ["code"] = code };
        void Add<T>(string field, T old, T @new)
        {
            if (!EqualityComparer<T>.Default.Equals(old, @new))
            {
                changes[field] = new object?[] { old, @new };
            }
        }

        Add("name", before.Name, after.Name);
        Add("monthlyPriceMinor", before.MonthlyPriceMinor, after.MonthlyPriceMinor);
        Add("yearlyPriceMinor", before.YearlyPriceMinor, after.YearlyPriceMinor);
        Add("includedBranchCount", before.IncludedBranchCount, after.IncludedBranchCount);
        Add("extraBranchPriceMinor", before.ExtraBranchPriceMinor, after.ExtraBranchPriceMinor);
        Add("staffLimit", before.StaffLimit, after.StaffLimit);
        Add("includedSmsCredits", before.IncludedSmsCredits, after.IncludedSmsCredits);
        Add("isPublic", before.IsPublic, after.IsPublic);
        Add("isActive", before.IsActive, after.IsActive);
        if (before.Features != after.Features)
        {
            changes["features"] = true;
        }

        if (!string.Equals(before.Description, after.Description, StringComparison.Ordinal))
        {
            changes["description"] = true;
        }

        return changes;
    }

    private static string? Text(string? value, string field, int min, int max, Dictionary<string, string[]> errors)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            errors[field] = ["required"];
            return null;
        }

        if (trimmed.Length < min || trimmed.Length > max)
        {
            errors[field] = ["out_of_range"];
            return null;
        }

        return trimmed;
    }

    private static long? MoneyMinor(MoneyInput? input, string field, Dictionary<string, string[]> errors)
    {
        if (input is null)
        {
            return null;
        }

        if (input.AmountMinor is not { } amount || input.Currency is null)
        {
            errors[field] = ["required"];
            return null;
        }

        if (!string.Equals(input.Currency, nameof(Currency.TRY), StringComparison.Ordinal))
        {
            errors[$"{field}.currency"] = ["invalid_format"];
            return null;
        }

        if (amount < 0)
        {
            errors[$"{field}.amountMinor"] = ["out_of_range"];
            return null;
        }

        return amount;
    }

    private static void Minimum(int? value, int min, string field, Dictionary<string, string[]> errors)
    {
        if (value < min)
        {
            errors[field] = ["out_of_range"];
        }
    }

    private static Money? Price(long? minor) => minor is { } m ? Money.Try(m) : null;

    private static uint Sum(IEnumerable<uint> versions) => versions.Aggregate(0u, (sum, v) => unchecked(sum + v));
}
