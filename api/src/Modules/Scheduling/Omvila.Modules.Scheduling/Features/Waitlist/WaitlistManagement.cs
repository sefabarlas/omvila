using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>
/// Panel bekleme listesi (sözleşme manage-waitlist; BeklemeListesi, BeklemeListesiBos): liste ve özet, "Listeye ekle",
/// "Saat öner", "Teklifi geri al". İzin waitlist.manage (Owner, BranchManager kendi şubesinde); şube kapsam dışındaysa 404.
/// </summary>
internal sealed class WaitlistManagement(
    SchedulingDbContext db,
    WaitlistInput input,
    WaitlistViewMapper mapper,
    WaitlistOfferEngine engine,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    IIdGenerator ids,
    IClock clock)
{
    private const int NameMinLength = 2;
    private const int NameMaxLength = 120;

    // Kural: sözleşme manageListWaitlist (öncelik ve kayıt sırası = teklifin gideceği sıra)
    public async Task<Result<WaitlistList>> ListAsync(Guid brandId, Guid? branchId, ManageAccess access, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        if (branchId is not { } branch)
        {
            return Error.Validation("branchId", "required");
        }

        var zoneId = await BranchZoneAsync(brandId, branch, access, ct);
        if (zoneId is null)
        {
            return Error.NotFound();
        }

        var now = clock.GetCurrentInstant();
        var zone = DateTimeZoneProviders.Tzdb[zoneId];
        var monthStart = zone.AtStartOfDay(now.InZone(zone).Date.With(DateAdjusters.StartOfMonth)).ToInstant();
        var waiting = await db.WaitlistEntries.AsNoTracking()
            .Include(e => e.Offers)
            .Where(e => e.BranchId == branch && e.Status == WaitlistEntryStatus.Waiting)
            .OrderByDescending(e => e.Priority).ThenBy(e => e.JoinedUtc).ThenBy(e => e.Id)
            .ToListAsync(ct);
        var converted = await db.WaitlistEntries.AsNoTracking()
            .CountAsync(e => e.BranchId == branch && e.Status == WaitlistEntryStatus.Converted && e.ClosedUtc >= monthStart, ct);
        var settings = await tenants.GetAppointmentSettingsAsync(brandId, ct);
        var items = await mapper.MapAsync(brandId, branch, waiting, ShowPhone(access, settings), ct);
        return new WaitlistList(
            branch,
            zoneId,
            now,
            items,
            new WaitlistOverview(waiting.Count, waiting.Count == 0 ? null : waiting.Min(e => e.JoinedUtc), converted, PlatformDefaults.WaitlistOfferHoldMinutes));
    }

    // Kural: sözleşme manageAddToWaitlist (Source = Panel; müşteriye bildirim yok)
    public async Task<Result<WaitlistEntry>> AddAsync(Guid brandId, ManageWaitlistCreate body, ManageAccess access, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(access);
        var preferences = WaitlistInput.Parse(body.Preferences);
        var errors = preferences.IsSuccess ? new Dictionary<string, string[]>(StringComparer.Ordinal) : new Dictionary<string, string[]>(preferences.Error.FieldErrors, StringComparer.Ordinal);
        var phone = ValidateCustomer(body, errors);
        if (body.BranchId is null)
        {
            errors["branchId"] = ["required"];
        }

        if (body.Note is { Length: > WaitlistEntry.NoteMaxLength })
        {
            errors["note"] = ["too_long"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var branchId = body.BranchId!.Value;
        if (await BranchZoneAsync(brandId, branchId, access, ct) is null)
        {
            return Error.NotFound();
        }

        var checkedPreferences = await input.CheckAsync(brandId, branchId, preferences.Value, online: false, ct);
        if (!checkedPreferences.IsSuccess)
        {
            return checkedPreferences.Error;
        }

        var customer = await ResolveCustomerAsync(brandId, body, phone, ct);
        if (!customer.IsSuccess)
        {
            return customer.Error;
        }

        var entry = WaitlistEntry.Create(
            ids.New(), brandId, branchId, customer.Value.CustomerId, customer.Value.BrandCustomerId, preferences.Value, body.Note,
            WaitlistSource.Panel, AccessCodeGenerator.New(), clock.GetCurrentInstant(), userId);
        db.WaitlistEntries.Add(entry);
        await db.SaveChangesAsync(ct);
        return entry;
    }

    // Kural: sözleşme manageOfferSlot, durum-makinesi.md § 3 (— → Pending)
    public async Task<Result<WaitlistEntry>> OfferAsync(
        Guid entryId, WaitlistOfferRequest body, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.StartUtc is null)
        {
            errors["startUtc"] = ["required"];
        }

        if (body.StaffId is null)
        {
            errors["staffId"] = ["required"];
        }

        var entry = await LoadAsync(entryId, access, ct);
        if (entry is null)
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(entry, entry.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var offered = await engine.OfferManuallyAsync(entry, body.StaffId!.Value, body.StartUtc!.Value, userId, ct);
        return offered.IsSuccess ? entry : offered.Error;
    }

    // Kural: sözleşme manageWithdrawWaitlistOffer, durum-makinesi.md § 3 (Pending → Withdrawn)
    public async Task<Result<WaitlistEntry>> WithdrawAsync(Guid entryId, ManageAccess access, uint expectedVersion, CancellationToken ct)
    {
        var entry = await LoadAsync(entryId, access, ct);
        if (entry is null)
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(entry, entry.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var withdrawn = entry.WithdrawOffer(clock.GetCurrentInstant());
        if (!withdrawn.IsSuccess)
        {
            return withdrawn.Error;
        }

        await db.SaveChangesAsync(ct);
        return entry;
    }

    public async Task<WaitlistEntryView> MapAsync(Guid brandId, WaitlistEntry entry, ManageAccess access, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var settings = await tenants.GetAppointmentSettingsAsync(brandId, ct);
        return (await mapper.MapAsync(brandId, entry.BranchId, [entry], ShowPhone(access, settings), ct))[0];
    }

    private async Task<WaitlistEntry?> LoadAsync(Guid entryId, ManageAccess access, CancellationToken ct)
    {
        var entry = await db.WaitlistEntries.Include(e => e.Offers).SingleOrDefaultAsync(e => e.Id == entryId, ct);
        return entry is not null && access.Branches.Includes(entry.BranchId) ? entry : null;
    }

    private async Task<string?> BranchZoneAsync(Guid brandId, Guid branchId, ManageAccess access, CancellationToken ct) =>
        access.Branches.Includes(branchId) ? await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) : null;

    private static bool ShowPhone(ManageAccess access, BrandAppointmentSettings? settings) =>
        access.HasPhoneView || settings is not { MaskCustomerPhoneFromStaff: true };

    private async Task<Result<(Guid CustomerId, Guid BrandCustomerId)>> ResolveCustomerAsync(
        Guid brandId, ManageWaitlistCreate body, PhoneNumber? phone, CancellationToken ct)
    {
        if (body.BrandCustomerId is { } brandCustomerId)
        {
            var info = await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct);
            return info.TryGetValue(brandCustomerId, out var card) ? (card.CustomerId, card.BrandCustomerId) : Error.NotFound();
        }

        var ensured = await customers.EnsureBrandCustomerAsync(
            new EnsureBrandCustomerRequest(brandId, phone!, body.NewCustomer!.FullName!.Trim(), CustomerSource.Panel, PhoneVerified: false), ct);
        if (!ensured.Created)
        {
            return Error.Conflict(
                ErrorCodes.PhoneAlreadyExists,
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["brandCustomerId"] = ensured.BrandCustomerId });
        }

        return (ensured.CustomerId, ensured.BrandCustomerId);
    }

    /// <summary>brandCustomerId ile newCustomer'dan tam biri (XOR).</summary>
    private static PhoneNumber? ValidateCustomer(ManageWaitlistCreate body, Dictionary<string, string[]> errors)
    {
        if ((body.BrandCustomerId is null) == (body.NewCustomer is null))
        {
            errors[body.BrandCustomerId is null ? "brandCustomerId" : "newCustomer"] = [body.BrandCustomerId is null ? "required" : "invalid_format"];
            return null;
        }

        if (body.NewCustomer is not { } newCustomer)
        {
            return null;
        }

        var name = newCustomer.FullName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["newCustomer.fullName"] = ["required"];
        }
        else if (name.Length is < NameMinLength or > NameMaxLength)
        {
            errors["newCustomer.fullName"] = ["out_of_range"];
        }

        if (newCustomer.Phone is null)
        {
            errors["newCustomer.phone"] = ["required"];
        }
        else if (PhoneNumber.TryParse(newCustomer.Phone, out var phone))
        {
            return phone;
        }
        else
        {
            errors["newCustomer.phone"] = ["invalid_format"];
        }

        return null;
    }
}
