using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Infrastructure;

internal sealed class StaffServiceAssignments(CatalogDbContext db, IIdGenerator ids) : IStaffServiceAssignments
{
    public async Task<IReadOnlyList<StaffServiceAssignment>> GetAsync(Guid brandId, Guid staffId, CancellationToken ct)
    {
        var rows = await (from x in db.ServiceStaff.AsNoTracking()
                          join s in db.Services.AsNoTracking() on x.ServiceId equals s.Id
                          where x.BrandId == brandId && x.StaffId == staffId && x.IsActive
                          orderby s.SortOrder, s.Name
                          select new { x, s.CurrencyCode }).ToListAsync(ct);
        return [.. rows.Select(r => new StaffServiceAssignment(
            r.x.ServiceId,
            r.x.DurationOverrideMinutes,
            r.x.SurchargeMinor > 0 ? new Money(r.x.SurchargeMinor, Enum.Parse<Currency>(r.CurrencyCode)) : null,
            r.x.CommissionPercentOverride))];
    }

    public async Task<IReadOnlyList<StaffServiceCommission>> GetCommissionOverridesAsync(
        Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct)
    {
        if (staffIds.Count == 0)
        {
            return [];
        }

        return await db.ServiceStaff.AsNoTracking()
            .Where(x => x.BrandId == brandId && x.IsActive && staffIds.Contains(x.StaffId) && x.CommissionPercentOverride != null)
            .Select(x => new StaffServiceCommission(x.StaffId, x.ServiceId, x.CommissionPercentOverride!.Value))
            .ToListAsync(ct);
    }

    public async Task<Result> SetAsync(
        Guid brandId, Guid staffId, IReadOnlyList<StaffServiceAssignment> assignments, string fieldPrefix, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(assignments);
        var serviceIds = assignments.Select(a => a.ServiceId).ToList();
        var services = await db.Services.AsNoTracking().Where(s => s.BrandId == brandId && s.IsActive && serviceIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, ct);
        var stages = (await db.Stages.AsNoTracking().Where(st => serviceIds.Contains(st.ServiceId)).ToListAsync(ct)).ToLookup(st => st.ServiceId);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var seen = new HashSet<Guid>();
        for (var i = 0; i < assignments.Count; i++)
        {
            var a = assignments[i];
            var field = $"{fieldPrefix}[{i}]";
            if (!services.TryGetValue(a.ServiceId, out var service) || !seen.Add(a.ServiceId))
            {
                errors[$"{field}.serviceId"] = ["invalid_format"];
                continue;
            }

            var serviceStages = stages[a.ServiceId].OrderBy(st => st.SeqNo).Select(st => st.ToStage()).ToList();
            if (a.DurationOverrideMinutes is { } minutes
                && (minutes > ServiceRules.MaxDurationMinutes || !OfferResolver.FitsStages(serviceStages, service.DurationMinutes, minutes)))
            {
                errors[$"{field}.durationOverrideMinutes"] = ["out_of_range"];
            }

            if (a.Surcharge is { } surcharge && surcharge.Currency != service.BasePrice.Currency)
            {
                errors[$"{field}.surcharge.currency"] = ["invalid_format"];
            }

            if (a.CommissionPercentOverride is < 0 or > 100)
            {
                errors[$"{field}.commissionPercentOverride"] = ["out_of_range"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var wanted = assignments.ToDictionary(a => a.ServiceId);
        await db.ServiceStaff.Where(x => x.BrandId == brandId && x.StaffId == staffId && !serviceIds.Contains(x.ServiceId)).ExecuteDeleteAsync(ct);
        var existing = await db.ServiceStaff.Where(x => x.BrandId == brandId && x.StaffId == staffId).ToListAsync(ct);
        foreach (var row in existing)
        {
            var a = wanted[row.ServiceId];
            row.SetOverrides(a.DurationOverrideMinutes, a.Surcharge?.AmountMinor ?? 0, a.CommissionPercentOverride);
        }

        foreach (var a in assignments.Where(a => existing.TrueForAll(r => r.ServiceId != a.ServiceId)))
        {
            var row = ServiceStaff.Create(ids.New(), brandId, a.ServiceId, staffId);
            row.SetOverrides(a.DurationOverrideMinutes, a.Surcharge?.AmountMinor ?? 0, a.CommissionPercentOverride);
            db.ServiceStaff.Add(row);
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
