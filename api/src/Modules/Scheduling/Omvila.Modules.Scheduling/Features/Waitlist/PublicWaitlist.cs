using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Features.PublicBooking;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>
/// Herkese açık bekleme listesi (sözleşme public-waitlist): katılma (telefon doğrulamasından sonra), teklif bağlantısı
/// (<c>/b/{code}</c>: görüntüle, "Randevuyu al", "Bu saat olmaz") ve "Listeden çık". Teklif kodu yalnız o teklife, kayıt
/// kodu yalnız o kaydın listeden çıkışına yetki verir (roller-yetkiler.md § erişim kodu). Kabulde randevu online
/// kurallarıyla <see cref="AppointmentWriter"/> üzerinden yazılır (A1; W4: onay veya kapora isteyen salonda Onay bekliyor).
/// </summary>
internal sealed class PublicWaitlist(
    SchedulingDbContext db,
    IDbSession session,
    WaitlistInput input,
    WaitlistViewMapper mapper,
    WaitlistOfferEngine engine,
    AppointmentWriter writer,
    ICustomerDirectory customers,
    IServiceOffers offers,
    IServiceDeposits deposits,
    IPublicBookingQueries booking,
    ITenantQueries tenants,
    IBrandBankAccounts bankAccounts,
    IOutbox outbox,
    IIdGenerator ids,
    IClock clock)
{
    private const int NameMinLength = 2;
    private const int NameMaxLength = 120;
    private const int DocumentVersionMaxLength = 20;

    /// <summary>
    /// "Bekleme listesine katıl": aynı müşterinin aynı şubede aynı hizmetler için bekleyen kaydı varsa tercihleri güncellenir
    /// (Created false). Engellenmiş müşteri 403 <c>customer_blocked</c>.
    /// </summary>
    // Kural: sözleşme publicJoinWaitlist; guvenlik-kvkk.md § Rıza (aydınlatma metni)
    public async Task<Result<(WaitlistEntry Entry, bool Created)>> JoinAsync(PublicBookingRequest context, PublicWaitlistJoin body, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(body);
        var brandId = context.Scope.BrandId;
        var branchId = context.Scope.BranchId;
        var valid = Validate(body);
        if (!valid.IsSuccess)
        {
            return valid.Error;
        }

        var (preferences, name, version) = valid.Value;
        var checkedPreferences = await input.CheckAsync(brandId, branchId, preferences, online: true, ct);
        if (!checkedPreferences.IsSuccess)
        {
            return checkedPreferences.Error;
        }

        var customer = await customers.EnsureBrandCustomerAsync(
            new EnsureBrandCustomerRequest(brandId, context.Phone, name, CustomerSource.Online, PhoneVerified: true), ct);
        if (customer.IsBlocked)
        {
            return Error.Forbidden(ErrorCodes.CustomerBlocked);
        }

        var now = clock.GetCurrentInstant();
        await customers.RecordConsentAsync(
            new ConsentEntry(customer.CustomerId, brandId, ConsentType.PrivacyNoticeAcknowledged, IsGranted: true, version, context.ConsentChannel, context.IpAddress, now), ct);
        var waiting = await db.WaitlistEntries
            .Where(e => e.BranchId == branchId && e.BrandCustomerId == customer.BrandCustomerId && e.Status == WaitlistEntryStatus.Waiting)
            .ToListAsync(ct);
        if (waiting.Find(e => e.Preferences.SameServices(preferences.ServiceIds)) is { } existing)
        {
            existing.UpdatePreferences(preferences, now);
            await db.SaveChangesAsync(ct);
            return (existing, false);
        }

        var entry = WaitlistEntry.Create(
            ids.New(), brandId, branchId, customer.CustomerId, customer.BrandCustomerId, preferences, note: null,
            WaitlistSource.Online, AccessCodeGenerator.New(), now);
        db.WaitlistEntries.Add(entry);
        await db.SaveChangesAsync(ct);
        return (entry, true);
    }

    /// <summary>Biçim denetimi (sözleşme PublicWaitlistJoin).</summary>
    private static Result<(WaitlistPreferences Preferences, string Name, string Version)> Validate(PublicWaitlistJoin body)
    {
        var preferences = WaitlistInput.Parse(body.Preferences);
        var errors = preferences.IsSuccess ? new Dictionary<string, string[]>(StringComparer.Ordinal) : new Dictionary<string, string[]>(preferences.Error.FieldErrors, StringComparer.Ordinal);
        var name = body.FullName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["fullName"] = ["required"];
        }
        else if (name.Length is < NameMinLength or > NameMaxLength)
        {
            errors["fullName"] = ["out_of_range"];
        }

        if (body.PrivacyNoticeAcknowledged != true)
        {
            errors["privacyNoticeAcknowledged"] = ["required"];
        }

        var version = body.PrivacyNoticeVersion?.Trim();
        if (string.IsNullOrEmpty(version))
        {
            errors["privacyNoticeVersion"] = ["required"];
        }
        else if (version.Length > DocumentVersionMaxLength)
        {
            errors["privacyNoticeVersion"] = ["too_long"];
        }

        return errors.Count > 0 ? Error.Validation(errors) : (preferences.Value, name!, version!);
    }

    /// <summary>
    /// Teklif koduyla kayıt ve teklif; oturum kaydın markasına geçer. Kod tablosu marka RLS'lidir ve markası bilinmez: kod
    /// platform bağlamında aranır (AppointmentLinkResolver deseni). Bilinmeyen kod null (404 <c>access_code_invalid</c>).
    /// </summary>
    public async Task<(WaitlistEntry Entry, WaitlistOffer Offer)?> EnterOfferAsync(string? code, CancellationToken ct)
    {
        if (!AccessCodeGenerator.IsWellFormed(code))
        {
            return null;
        }

        var found = await FindAsync(db.WaitlistOffers.Where(o => o.AccessCode == code).Select(o => new Located(o.BrandId, o.WaitlistEntryId)), ct);
        if (found is null)
        {
            return null;
        }

        var entry = await LoadAsync(found, ct);
        return entry?.Offers.FirstOrDefault(o => string.Equals(o.AccessCode, code, StringComparison.Ordinal)) is { } offer ? (entry, offer) : null;
    }

    /// <summary>Kayıt koduyla kayıt ("Listeden çık"); oturum kaydın markasına geçer.</summary>
    public async Task<WaitlistEntry?> EnterEntryAsync(string? code, CancellationToken ct)
    {
        if (!AccessCodeGenerator.IsWellFormed(code))
        {
            return null;
        }

        var found = await FindAsync(db.WaitlistEntries.Where(e => e.AccessCode == code).Select(e => new Located(e.BrandId, e.Id)), ct);
        return found is null ? null : await LoadAsync(found, ct);
    }

    /// <summary>
    /// "Randevuyu al". Kabul edilmiş teklif tekrar gelirse aynı randevu döner. Süre dolmuşsa <c>hold_expired</c>; reddedilmiş
    /// veya geri alınmışsa <c>invalid_state_transition</c>; engellenmiş müşteri 403 <c>customer_blocked</c>.
    /// </summary>
    // Kural: durum-makinesi.md § 3 (Pending → Accepted), A1 online kuralları (W4)
    public async Task<Result<(Appointment Appointment, string AccessCode)>> AcceptAsync(
        WaitlistEntry entry, WaitlistOffer offer, uint expectedVersion, ClientApp clientApp, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(offer);
        if (offer is { Status: WaitlistOfferStatus.Accepted, AppointmentId: { } acceptedId })
        {
            return await ExistingAsync(acceptedId, ct);
        }

        if (!db.MatchesVersion(offer, offer.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var now = clock.GetCurrentInstant();
        var can = entry.CanAccept(offer, now);
        if (!can.IsSuccess)
        {
            return can.Error;
        }

        var person = (await customers.GetDisplayInfoAsync(entry.BrandId, [entry.BrandCustomerId], ct)).GetValueOrDefault(entry.BrandCustomerId);
        if (person is null)
        {
            return Error.NotFound();
        }

        if (person.IsBlocked)
        {
            return Error.Forbidden(ErrorCodes.CustomerBlocked);
        }

        var settings = await tenants.GetAppointmentSettingsAsync(entry.BrandId, ct) ?? throw new DomainException("Markanın randevu ayarları yok.");
        var slot = new SlotRequest(
            entry.BranchId, offer.OfferedStartUtc, [.. entry.ServiceIds.Select(s => new RequestedItem(s, offer.OfferedStaffId))], SameStaffForAll: true, AvailabilityMode.Public);
        var written = await writer.CreateAsync(
            new CreateAppointmentCommand(
                entry.BrandId, slot, entry.CustomerId, entry.BrandCustomerId, AppointmentChannel.Online, clientApp, CustomerNote: null,
                StaffNote: null, SeriesId: null, CreatedByUserId: null, settings.Policy(),
                await PublicAppointmentCreator.DepositTermsAsync(bankAccounts, entry.BrandId, person.NoShowCount, ct)),
            ct);
        if (!written.IsSuccess)
        {
            return written.Error;
        }

        var appointment = written.Value.Appointment;
        var accepted = entry.Accept(offer, appointment.Id, now);
        if (!accepted.IsSuccess)
        {
            return accepted.Error;
        }

        await db.SaveChangesAsync(ct);
        await PublicAppointmentCreator.PublishCreatedAsync(outbox, appointment, ct);
        return (appointment, written.Value.AccessCode!.Code);
    }

    /// <summary>"Bu saat olmaz": teklif Declined, saat sıradaki uygun kişiye teklif edilir; kişi listede kalır.</summary>
    // Kural: durum-makinesi.md § 3 (Pending → Declined)
    public async Task<Result> DeclineAsync(WaitlistEntry entry, WaitlistOffer offer, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(offer);
        if (offer.Status != WaitlistOfferStatus.Declined && !db.MatchesVersion(offer, offer.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var declined = entry.Decline(offer, clock.GetCurrentInstant());
        if (!declined.IsSuccess)
        {
            return declined.Error;
        }

        if (declined.Value)
        {
            await db.SaveChangesAsync(ct);
            await engine.OfferFreedSlotAsync(entry.BrandId, entry.BranchId, offer.OfferedStaffId, offer.OfferedStartUtc, ct);
        }

        return Result.Success();
    }

    /// <summary>"Listeden çık": kayıt Cancelled; bekleyen teklif varsa Declined olur ve saat sıradakine geçer.</summary>
    // Kural: sözleşme publicLeaveWaitlist, durum-makinesi.md § 3 (Waiting → Cancelled)
    public async Task<Result> LeaveAsync(WaitlistEntry entry, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var wasCancelled = entry.Status == WaitlistEntryStatus.Cancelled;
        var left = entry.Leave(clock.GetCurrentInstant());
        if (!left.IsSuccess)
        {
            return left.Error;
        }

        if (!wasCancelled)
        {
            await db.SaveChangesAsync(ct);
        }

        if (left.Value is { } released)
        {
            await engine.OfferFreedSlotAsync(entry.BrandId, entry.BranchId, released.OfferedStaffId, released.OfferedStartUtc, ct);
        }

        return Result.Success();
    }

    public async Task<PublicWaitlistEntry?> MapEntryAsync(WaitlistEntry entry, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var zone = await tenants.GetBranchTimeZoneAsync(entry.BrandId, entry.BranchId, ct);
        return zone is null ? null : await mapper.MapPublicAsync(entry, zone, ct);
    }

    /// <summary>Sözleşme PublicWaitlistOffer: kalemler ve fiyat teklif edilen personelle, kapora müşterinin gelmeme sayısıyla çözülür.</summary>
    public async Task<PublicWaitlistOffer?> MapOfferAsync(WaitlistEntry entry, WaitlistOffer offer, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(offer);
        var card = await booking.GetBranchCardAsync(entry.BrandId, entry.BranchId, ct);
        var settings = await tenants.GetAppointmentSettingsAsync(entry.BrandId, ct);
        if (card is null || settings is null)
        {
            return null;
        }

        var person = (await customers.GetDisplayInfoAsync(entry.BrandId, [entry.BrandCustomerId], ct)).GetValueOrDefault(entry.BrandCustomerId);
        var resolved = await offers.ResolveAsync(
            entry.BrandId, entry.BranchId, [.. entry.ServiceIds.Select(s => new ServiceOfferRequest(s, offer.OfferedStaffId))], ct);
        var resolvedOffers = resolved.Where(r => r.IsSuccess).Select(r => r.Value).ToList();
        var staff = await PublicAvailabilityEndpoints.PublicNamesAsync(tenants, entry.BrandId, [offer.OfferedStaffId], ct);
        var staffName = staff.TryGetValue(offer.OfferedStaffId, out var staffCard) ? staffCard.FullName : null;
        var currency = resolvedOffers.FirstOrDefault()?.Price.Currency ?? PlatformDefaults.DefaultCurrency;
        var total = resolvedOffers.Sum(o => o.Price.AmountMinor);
        var deposit = await DepositAsync(entry, person, resolvedOffers, ct);
        var requiresApproval = deposit > 0 || resolvedOffers.Exists(o => o.RequiresApproval ?? settings.RequiresApproval);
        var appointmentCode = offer.AppointmentId is { } appointmentId
            ? await db.AppointmentAccessCodes.AsNoTracking()
                .Where(c => c.AppointmentId == appointmentId && c.Purpose == AccessCodePurpose.Manage && c.RevokedUtc == null)
                .Select(c => c.Code).FirstOrDefaultAsync(ct)
            : null;
        return new PublicWaitlistOffer(
            offer.Id,
            offer.Status.ToString(),
            new PublicWaitlistOffer.BrandView(card.BrandName, card.BrandSlug, card.LogoUrl),
            new PublicWaitlistOffer.BranchView(card.BranchId, card.BranchName),
            person?.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
            offer.OfferedStartUtc,
            offer.OfferedEndUtc,
            card.TimeZoneId,
            offer.HoldExpiresUtc,
            clock.GetCurrentInstant(),
            [.. resolvedOffers.Select(o => new PublicWaitlistOffer.ItemView(o.ServiceName, staffName, o.DurationMinutes, o.Price))],
            new Money(total, currency),
            new Money(total - deposit, currency),
            requiresApproval,
            deposit > 0 ? new Money(deposit, currency) : null,
            appointmentCode,
            entry.AccessCode,
            ETagVersion.Format(offer.Version));
    }

    private async Task<long> DepositAsync(WaitlistEntry entry, CustomerDisplayInfo? person, List<ServiceOffer> resolved, CancellationToken ct)
    {
        if (resolved.Count == 0 || await bankAccounts.GetActiveAsync(entry.BrandId, ct) is null)
        {
            return 0;
        }

        var amounts = await deposits.ResolveAsync(
            entry.BrandId,
            new ServiceDepositRequest(IsOnlineChannel: true, person?.NoShowCount ?? 0, [.. resolved.Select(o => new ServiceDepositItem(o.ServiceId, o.Price, IsPackageRedemption: false))]),
            ct);
        return amounts.Sum(d => d?.AmountMinor ?? 0);
    }

    private async Task<Result<(Appointment Appointment, string AccessCode)>> ExistingAsync(Guid appointmentId, CancellationToken ct)
    {
        var appointment = await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleOrDefaultAsync(a => a.Id == appointmentId, ct);
        var code = await db.AppointmentAccessCodes.AsNoTracking()
            .Where(c => c.AppointmentId == appointmentId && c.Purpose == AccessCodePurpose.Manage && c.RevokedUtc == null)
            .Select(c => c.Code).FirstOrDefaultAsync(ct);
        return appointment is null || code is null ? Error.NotFound() : (appointment, code);
    }

    private async Task<Located?> FindAsync(IQueryable<Located> query, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await query.AsNoTracking().SingleOrDefaultAsync(ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }

    private async Task<WaitlistEntry?> LoadAsync(Located located, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.ForBrand(located.BrandId), ct);
        return await db.WaitlistEntries.Include(e => e.Offers).SingleOrDefaultAsync(e => e.Id == located.EntryId, ct);
    }

    private sealed record Located(Guid BrandId, Guid EntryId);
}
