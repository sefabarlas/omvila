using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.PublicBooking;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>
/// Müşteri uygulaması randevuları (sözleşme meListAppointments, meGetAppointment; dilim-05 KK2, KK8). Randevular birden çok
/// markada olduğundan liste platform bağlamında yalnız müşterinin global kimliğiyle süzülerek okunur (T-103 deseni); tek randevu
/// için kimlik platform bağlamında doğrulanır, sonra oturum randevunun markasına geçer (değişiklikler markanın yazma yolunda).
/// Başka müşterinin randevusu ve taslak (online tutma) bulunamaz.
/// </summary>
internal sealed class MeAppointmentReader(
    SchedulingDbContext db,
    IDbSession session,
    IPublicBookingQueries booking,
    ITenantQueries tenants,
    ICustomerBrandDirectory brands,
    IServiceOffers offers,
    IBrandBankAccounts bankAccounts,
    ICustomerReviewSummaries reviews,
    IClock clock)
{
    public const string UpcomingScope = "Upcoming";
    public const string PastScope = "Past";

    private static readonly AppointmentStatus[] OpenStatuses =
        [AppointmentStatus.PendingApproval, AppointmentStatus.Confirmed, AppointmentStatus.CheckedIn];

    /// <summary>Randevuyu (izlenen, kalemler ve dilimlerle) yükler ve oturumu markasına kurar; müşterinin değilse null.</summary>
    public async Task<Appointment?> EnterAsync(Guid customerId, Guid appointmentId, CancellationToken ct)
    {
        var brandId = await InPlatformAsync(
            () => db.Appointments.AsNoTracking()
                .Where(a => a.Id == appointmentId && a.CustomerId == customerId && a.Status != AppointmentStatus.Draft)
                .Select(a => (Guid?)a.BrandId)
                .SingleOrDefaultAsync(ct),
            ct);
        if (brandId is null)
        {
            return null;
        }

        await session.SetTenantAsync(TenantScope.ForBrand(brandId.Value), ct);
        return await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleAsync(a => a.Id == appointmentId, ct);
    }

    // Kural: sözleşme meListAppointments (Upcoming: açık durum ve bitişi gelmemiş, artan; Past: kalanlar, azalan; notReviewed yalnız Past)
    public async Task<Result<MeAppointmentPage>> ListAsync(Guid customerId, string? scope, bool notReviewed, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (scope is null)
        {
            errors["scope"] = ["required"];
        }
        else if (scope is not (UpcomingScope or PastScope))
        {
            errors["scope"] = ["invalid_format"];
        }
        else if (notReviewed && string.Equals(scope, UpcomingScope, StringComparison.Ordinal))
        {
            errors["notReviewed"] = ["invalid_format"];
        }

        if (!MeCursor.TryLimit(limit, out var take))
        {
            errors["limit"] = ["out_of_range"];
        }

        if (!MeCursor.TryDecode(cursor, out var offset))
        {
            errors["cursor"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        return await InPlatformAsync(() => PageAsync(customerId, string.Equals(scope, UpcomingScope, StringComparison.Ordinal), notReviewed, take, offset, ct), ct);
    }

    /// <summary>Sözleşme MeAppointment. Oturum randevunun markasında olmalıdır (<see cref="EnterAsync"/>).</summary>
    public async Task<MeAppointmentView?> MapAsync(Appointment appointment, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        var card = await booking.GetBranchCardAsync(appointment.BrandId, appointment.BranchId, ct);
        var settings = await tenants.GetAppointmentSettingsAsync(appointment.BrandId, ct);
        if (card is null || settings is null)
        {
            return null;
        }

        var context = await ContextAsync([appointment], ct);
        var account = appointment.DisplayStatus == DisplayStatus.DepositPending ? await bankAccounts.GetActiveAsync(appointment.BrandId, ct) : null;
        return MeAppointmentMapper.Detail(appointment, card, settings, context, account, clock.GetCurrentInstant());
    }

    private async Task<MeAppointmentPage> PageAsync(Guid customerId, bool upcoming, bool notReviewed, int take, int offset, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var mine = db.Appointments.AsNoTracking().Where(a => a.CustomerId == customerId && a.Status != AppointmentStatus.Draft);
        var open = mine.Where(a => OpenStatuses.Contains(a.Status) && a.EndUtc > now);
        var query = upcoming
            ? open.OrderBy(a => a.StartUtc).ThenBy(a => a.Id)
            : mine.Where(a => !(OpenStatuses.Contains(a.Status) && a.EndUtc > now)).OrderByDescending(a => a.StartUtc).ThenByDescending(a => a.Id);
        List<Guid> ids;
        if (notReviewed)
        {
            // "Değerlendirilmedi": tamamlanmış ve değerlendirilmemiş; değerlendirmeler Reviews'tadır, süzme bellekte.
            var completed = await query.Where(a => a.Status == AppointmentStatus.Completed).Select(a => a.Id).ToListAsync(ct);
            var reviewed = await reviews.GetByAppointmentsAsync(completed, ct);
            ids = [.. completed.Where(id => !reviewed.ContainsKey(id)).Skip(offset).Take(take + 1)];
        }
        else
        {
            ids = await query.Select(a => a.Id).Skip(offset).Take(take + 1).ToListAsync(ct);
        }

        var pageIds = ids.Take(take).ToList();
        var loaded = await db.Appointments.AsNoTracking().Include(a => a.Items).Where(a => pageIds.Contains(a.Id)).ToListAsync(ct);
        var ordered = pageIds.Select(id => loaded.Single(a => a.Id == id)).ToList();
        var context = await ContextAsync(ordered, ct);
        var cards = new Dictionary<(Guid, Guid), PublicBranchCard>();
        foreach (var key in ordered.Select(a => (a.BrandId, a.BranchId)).Distinct())
        {
            if (await booking.GetBranchCardAsync(key.BrandId, key.BranchId, ct) is { } card)
            {
                cards[key] = card;
            }
        }

        var rows = ordered.Where(a => cards.ContainsKey((a.BrandId, a.BranchId)))
            .Select(a => MeAppointmentMapper.Summary(a, cards[(a.BrandId, a.BranchId)], context))
            .ToList();
        return new MeAppointmentPage(rows, ids.Count > take ? MeCursor.Encode(offset + take) : null);
    }

    /// <summary>Personel adları, değerlendirme özetleri ve "Aynısını tekrar al" (KK2) uygunluğu, toplu.</summary>
    private async Task<MeAppointmentContext> ContextAsync(IReadOnlyList<Appointment> appointments, CancellationToken ct)
    {
        var staff = new Dictionary<Guid, StaffCard>();
        foreach (var group in appointments.GroupBy(a => a.BrandId))
        {
            var staffIds = group.SelectMany(a => a.Items).Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
            foreach (var (id, card) in await PublicAvailabilityEndpoints.PublicNamesAsync(tenants, group.Key, staffIds, ct))
            {
                staff[id] = card;
            }
        }

        var reviewed = await reviews.GetByAppointmentsAsync([.. appointments.Select(a => a.Id)], ct);
        var brandCards = await brands.GetCardsAsync([.. appointments.Select(a => a.BrandId).Distinct()], ct);
        var rebookable = new HashSet<Guid>();
        foreach (var branch in appointments.GroupBy(a => (a.BrandId, a.BranchId)))
        {
            if (!brandCards.TryGetValue(branch.Key.BrandId, out var brand) || !brand.BookingEnabled)
            {
                continue;
            }

            var online = await OnlineServicesAsync(branch.Key.BrandId, branch.Key.BranchId, branch.SelectMany(a => a.Items), ct);
            rebookable.UnionWith(branch.Where(a => a.Items.Where(Visible(a)).All(i => online.Contains(i.ServiceId))).Select(a => a.Id));
        }

        return new MeAppointmentContext(staff, reviewed, rebookable);
    }

    // Kural: sözleşme MeAppointmentSummary.canRebook — kalemlerin hizmetleri şubede etkin ve online randevuya açık (personel aranmaz).
    private async Task<HashSet<Guid>> OnlineServicesAsync(Guid brandId, Guid branchId, IEnumerable<AppointmentItem> items, CancellationToken ct)
    {
        var serviceIds = items.Select(i => i.ServiceId).Distinct().ToList();
        var resolved = await offers.ResolveAsync(brandId, branchId, [.. serviceIds.Select(s => new ServiceOfferRequest(s, null))], ct);
        return [.. serviceIds.Zip(resolved).Where(x => x.Second is { IsSuccess: true, Value.IsOnlineBookable: true }).Select(x => x.First)];
    }

    /// <summary>İptal edilmiş kalem yalnız iptal edilmiş randevuda görünür (PublicAppointmentMapper ile aynı kural).</summary>
    internal static Func<AppointmentItem, bool> Visible(Appointment appointment) =>
        i => i.Status != AppointmentItemStatus.Cancelled || appointment.Status == AppointmentStatus.Cancelled;

    private async Task<T> InPlatformAsync<T>(Func<Task<T>> read, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await read();
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
