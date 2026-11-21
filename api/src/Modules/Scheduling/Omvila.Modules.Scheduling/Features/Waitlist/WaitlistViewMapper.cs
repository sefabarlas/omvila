using Omvila.Infrastructure.Http;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>
/// Bekleme kayıtlarını sözleşme görünümlerine eşler. Adlar toplu okunur (hizmet adları Catalog, personel adları Tenancy,
/// müşteri kartı Customers); döngüde sorgu yoktur.
/// </summary>
internal sealed class WaitlistViewMapper(IServiceOffers offers, ITenantQueries tenants, ICustomerDirectory customers)
{
    public async Task<IReadOnlyList<WaitlistEntryView>> MapAsync(
        Guid brandId, Guid branchId, IReadOnlyList<WaitlistEntry> entries, bool showPhone, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entries);
        if (entries.Count == 0)
        {
            return [];
        }

        var services = await ServiceNamesAsync(brandId, branchId, entries.SelectMany(e => e.ServiceIds), ct);
        var staffIds = entries.Select(e => e.PreferredStaffId)
            .Concat(entries.Select(e => e.PendingOffer?.OfferedStaffId))
            .Where(s => s is not null).Select(s => s!.Value).Distinct().ToList();
        var staff = staffIds.Count == 0 ? new Dictionary<Guid, StaffCard>() : await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var people = await customers.GetDisplayInfoAsync(brandId, [.. entries.Select(e => e.BrandCustomerId).Distinct()], ct);
        return [.. entries.Select(e =>
        {
            var person = people.GetValueOrDefault(e.BrandCustomerId);
            var offer = e.PendingOffer;
            return new WaitlistEntryView(
                e.Id,
                e.BranchId,
                e.Status.ToString(),
                new WaitlistCustomerRef(
                    e.BrandCustomerId,
                    person?.FullName ?? string.Empty,
                    showPhone && person?.PhoneMasked is not null ? person.PhoneE164 : null,
                    person?.PhoneMasked),
                e.ServiceIds,
                [.. e.ServiceIds.Select(s => services.GetValueOrDefault(s, string.Empty))],
                e.PreferredStaffId,
                StaffName(staff, e.PreferredStaffId),
                e.AcceptOtherStaff,
                e.Dates,
                [.. e.TimeWindows.Select(w => w.ToString())],
                e.Note,
                e.JoinedUtc,
                e.Source.ToString(),
                offer is null
                    ? null
                    : new WaitlistOfferView(
                        offer.Id, offer.Status.ToString(), offer.OfferedStartUtc, offer.OfferedEndUtc, offer.OfferedStaffId,
                        StaffName(staff, offer.OfferedStaffId), offer.OfferedUtc, offer.HoldExpiresUtc),
                ETagVersion.Format(e.Version));
        })];
    }

    public async Task<PublicWaitlistEntry> MapPublicAsync(WaitlistEntry entry, string timeZone, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var services = await ServiceNamesAsync(entry.BrandId, entry.BranchId, entry.ServiceIds, ct);
        var staff = entry.PreferredStaffId is { } staffId
            ? await tenants.GetStaffCardsAsync(entry.BrandId, [staffId], ct)
            : new Dictionary<Guid, StaffCard>();
        return new PublicWaitlistEntry(
            entry.Id,
            entry.AccessCode,
            entry.Status.ToString(),
            entry.BranchId,
            timeZone,
            [.. entry.ServiceIds.Select(s => services.GetValueOrDefault(s, string.Empty))],
            StaffName(staff, entry.PreferredStaffId),
            entry.AcceptOtherStaff,
            entry.Dates,
            [.. entry.TimeWindows.Select(w => w.ToString())]);
    }

    private async Task<Dictionary<Guid, string>> ServiceNamesAsync(Guid brandId, Guid branchId, IEnumerable<Guid> serviceIds, CancellationToken ct)
    {
        var ids = serviceIds.Distinct().ToList();
        var resolved = await offers.ResolveAsync(brandId, branchId, [.. ids.Select(s => new ServiceOfferRequest(s, null))], ct);
        var names = new Dictionary<Guid, string>();
        for (var i = 0; i < ids.Count; i++)
        {
            if (resolved[i].IsSuccess)
            {
                names[ids[i]] = resolved[i].Value.ServiceName;
            }
        }

        return names;
    }

    private static string? StaffName(IReadOnlyDictionary<Guid, StaffCard> staff, Guid? staffId) =>
        staffId is { } id && staff.TryGetValue(id, out var card) ? card.FullName : null;
}
