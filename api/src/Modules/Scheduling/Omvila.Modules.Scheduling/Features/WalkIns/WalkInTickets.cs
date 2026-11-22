using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>
/// Kapıda sıra yazma işleri (sözleşme manageCreateWalkIn, manageStartWalkIn, manageLeaveWalkIn; dilim-05 kabul kriteri 4).
/// "Başlat" randevuyu yalnız <see cref="AppointmentWriter"/> ile yazar (A1, kapıda kanal: doğrudan CheckedIn). Sıra ilerleyince
/// (başlatma veya sıradan çıkma) önünde 1 kişi kalan biletin telefonuna bir kez <c>walkin.almost_turn</c> SMS'i aynı
/// transaction'da outbox'a yazılır. Yeni bilet <see cref="WalkInTicketCreated"/> yayımlar (staff.walkin_queue, T-105).
/// </summary>
internal sealed class WalkInTickets(
    SchedulingDbContext db,
    BranchQueueNumbers numbers,
    AppointmentWriter writer,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    IServiceOffers offers,
    ISubscriptionQueries subscriptions,
    IPublicBookingQueries booking,
    INotificationOutbox notifications,
    IOutbox outbox,
    WalkInQueueReader queues,
    IIdGenerator ids,
    IClock clock)
{
    private const int NameMinLength = 2;
    private const int NameMaxLength = 120;

    // Kural: alan-modeli.md § WalkInTicket (numara şube + gün bazında 1'den; müşteri kaydı "Başlat"ta açılır)
    public async Task<Result<WalkInTicket>> CreateAsync(Guid brandId, WalkInCreate body, ManageAccess access, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(access);
        var input = Validate(body);
        if (!input.IsSuccess)
        {
            return input.Error;
        }

        var (branchId, name, phone) = input.Value;
        if (!access.Branches.Includes(branchId) || await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) is not { } zoneId)
        {
            return Error.NotFound();
        }

        if (body.BrandCustomerId is { } brandCustomerId)
        {
            var cards = await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct);
            if (!cards.TryGetValue(brandCustomerId, out var card))
            {
                return Error.NotFound();
            }

            // Var olan müşteride telefon girilmediyse kartın numarası (randevu SMS'lerine izin veriyorsa) kullanılır.
            phone ??= card.AppointmentSmsEnabled && PhoneNumber.TryParse(card.PhoneE164, out var cardPhone) ? cardPhone : null;
        }

        var service = await ResolveServiceAsync(brandId, branchId, body.ServiceId, body.PreferredStaffId, ct);
        if (!service.IsSuccess)
        {
            return service.Error;
        }

        var now = clock.GetCurrentInstant();
        var date = now.InZone(DateTimeZoneProviders.Tzdb[zoneId]).Date;
        var number = await numbers.NextAsync(brandId, branchId, date, ct);
        var ticket = WalkInTicket.Create(
            new NewWalkInTicket(
                brandId, branchId, name, phone, body.BrandCustomerId, body.ServiceId, service.Value?.ServiceName, service.Value?.DurationMinutes,
                body.PreferredStaffId, userId),
            ids.New(),
            date,
            number,
            now);
        db.WalkInTickets.Add(ticket);
        await db.SaveChangesAsync(ct);

        // Kural: bildirimler.md staff.walkin_queue ("Sırada {n} kişi · tahmini bekleme {dk} dk"); alıcı ve tercih Notifications'ta.
        var queue = await queues.LoadAsync(brandId, branchId, ct);
        var waiting = queue?.WaitingCount ?? 1;
        var wait = queue?.Items.SingleOrDefault(i => i.Id == ticket.Id)?.EstimatedWaitMinutes;
        await outbox.EnqueueAsync(new WalkInTicketCreated(brandId, branchId, ticket.Id, userId, waiting, wait), ct);
        return ticket;
    }

    /// <summary>
    /// "Başlat / Randevuya çevir". Müşteri: biletteki kart; yoksa telefonla bulunur veya açılır (Source = WalkIn); telefon da
    /// yoksa adla telefonsuz müşteri açılır (T-86 W6). Randevu şimdi (dakikaya yuvarlanmış) başlar; kapıdaki müşteri o an
    /// salonda olduğundan çalışma saati uyarısı engel değildir, çakışma (slot_taken) engeldir (T-91).
    /// </summary>
    // Kural: durum-makinesi.md A1 (kapıda sıra → CheckedIn) ve § 4 (Waiting/Called → Served)
    public async Task<Result<(WalkInTicket Ticket, Appointment Appointment)>> StartAsync(
        Guid brandId, Guid ticketId, WalkInStartRequest? body, ManageAccess access, ManageAccess createAccess, Guid? userId, uint? expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(createAccess);
        var loaded = await LoadAsync(ticketId, access, expectedVersion, ct);
        if (!loaded.IsSuccess)
        {
            return loaded.Error;
        }

        var ticket = loaded.Value;
        var target = Target(ticket, body, createAccess);
        if (!target.IsSuccess)
        {
            return target.Error;
        }

        var (serviceId, staffId) = target.Value;
        if (await subscriptions.GetBannerAsync(brandId, ct) is { BlocksNewWork: true })
        {
            return Error.Forbidden(ErrorCodes.SubscriptionReadOnly);
        }

        var customer = await ResolveCustomerAsync(brandId, ticket, ct);
        if (!customer.IsSuccess)
        {
            return customer.Error;
        }

        var now = clock.GetCurrentInstant();
        var start = Instant.FromUnixTimeTicks(now.ToUnixTimeTicks() / NodaConstants.TicksPerMinute * NodaConstants.TicksPerMinute);
        var slot = new SlotRequest(ticket.BranchId, start, [new RequestedItem(serviceId, staffId)], SameStaffForAll: false, AvailabilityMode.Panel,
            AllowOutsideWorkingHours: true);
        var written = await writer.CreateAsync(
            new CreateAppointmentCommand(
                brandId, slot, customer.Value.CustomerId, customer.Value.BrandCustomerId, AppointmentChannel.WalkIn, ClientApp.Panel, CustomerNote: null,
                StaffNote: null, SeriesId: null, userId),
            ct);
        if (!written.IsSuccess)
        {
            return written.Error;
        }

        var appointment = written.Value.Appointment;
        var served = ticket.Serve(appointment.Id, customer.Value.BrandCustomerId, now);
        if (!served.IsSuccess)
        {
            return served.Error!;
        }

        await db.SaveChangesAsync(ct);
        await NotifyAlmostTurnAsync(ticket, now, ct);
        return (ticket, appointment);
    }

    /// <summary>"Sıradan çıkar": bilet Left; arkadakilerin beklemesi okumada yeniden hesaplanır.</summary>
    // Kural: durum-makinesi.md § 4 (Waiting/Called → Left)
    public async Task<Result<WalkInTicket>> LeaveAsync(Guid ticketId, ManageAccess access, uint? expectedVersion, CancellationToken ct)
    {
        var loaded = await LoadAsync(ticketId, access, expectedVersion, ct);
        if (!loaded.IsSuccess)
        {
            return loaded.Error;
        }

        var ticket = loaded.Value;
        var now = clock.GetCurrentInstant();
        var left = ticket.Leave(now);
        if (!left.IsSuccess)
        {
            return left.Error!;
        }

        await db.SaveChangesAsync(ct);
        await NotifyAlmostTurnAsync(ticket, now, ct);
        return ticket;
    }

    /// <summary>
    /// Gövdede verilmeyen alan biletteki değerdir; personel "Fark etmez" veya hizmet yoksa ilgili alan zorunludur (400).
    /// Personel (K) yalnız kendine randevu açar (roller-yetkiler.md appointment.create "K (kendine)"; 403).
    /// </summary>
    private static Result<(Guid ServiceId, Guid StaffId)> Target(WalkInTicket ticket, WalkInStartRequest? body, ManageAccess createAccess)
    {
        var serviceId = body?.ServiceId ?? ticket.ServiceId;
        var staffId = body?.StaffId ?? ticket.PreferredStaffId;
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (serviceId is null)
        {
            errors["serviceId"] = ["required"];
        }

        if (staffId is null)
        {
            errors["staffId"] = ["required"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        return createAccess.IsOwnStaff(staffId) ? (serviceId!.Value, staffId!.Value) : Error.Forbidden();
    }

    /// <summary>Kapsam (404), sürüm (412) ve durum (409 invalid_state_transition) denetimi.</summary>
    private async Task<Result<WalkInTicket>> LoadAsync(Guid ticketId, ManageAccess access, uint? expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        var ticket = await db.WalkInTickets.SingleOrDefaultAsync(t => t.Id == ticketId, ct);
        if (ticket is null || !access.Branches.Includes(ticket.BranchId))
        {
            return Error.NotFound();
        }

        if (expectedVersion is { } expected && !db.MatchesVersion(ticket, ticket.Version, expected))
        {
            return Error.PreconditionFailed();
        }

        return ticket.IsInQueue ? ticket : Error.Conflict(ErrorCodes.InvalidStateTransition);
    }

    /// <summary>
    /// Kural: bildirimler.md walkin.almost_turn — sıra ilerleyince önünde 1 kişi kalan (sıradaki ikinci) bilete, telefon
    /// verildiyse, bir kez. Bilet oluşurken gönderilmez: kişi o an salondadır (T-91).
    /// </summary>
    private async Task NotifyAlmostTurnAsync(WalkInTicket advanced, Instant now, CancellationToken ct)
    {
        var next = await db.WalkInTickets
            .Where(t => t.BrandId == advanced.BrandId && t.BranchId == advanced.BranchId && t.QueueDate == advanced.QueueDate
                && (t.Status == WalkInTicketStatus.Waiting || t.Status == WalkInTicketStatus.Called))
            .OrderBy(t => t.QueueNumber)
            .Take(2)
            .ToListAsync(ct);
        if (next.Count < 2 || !next[1].TryMarkAlmostTurn(now)
            || await booking.GetBranchCardAsync(advanced.BrandId, advanced.BranchId, ct) is not { } branch)
        {
            return;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal) { ["salon"] = branch.BrandName };
        await notifications.EnqueueSmsAsync(new SmsNotification(NotificationEvents.WalkInAlmostTurn, next[1].PhoneE164!, values, advanced.BrandId), ct);
        await db.SaveChangesAsync(ct);
    }

    private async Task<Result<(Guid CustomerId, Guid BrandCustomerId)>> ResolveCustomerAsync(Guid brandId, WalkInTicket ticket, CancellationToken ct)
    {
        if (ticket.BrandCustomerId is { } brandCustomerId)
        {
            var info = await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct);
            return info.TryGetValue(brandCustomerId, out var card) ? (card.CustomerId, card.BrandCustomerId) : Error.NotFound();
        }

        var ensured = PhoneNumber.TryParse(ticket.PhoneE164, out var phone)
            ? await customers.EnsureBrandCustomerAsync(new EnsureBrandCustomerRequest(brandId, phone, ticket.CustomerName, CustomerSource.WalkIn, PhoneVerified: false), ct)
            : await customers.CreateWithoutPhoneAsync(brandId, ticket.CustomerName, CustomerSource.WalkIn, ct);
        return (ensured.CustomerId, ensured.BrandCustomerId);
    }

    /// <summary>Hizmet şubede verilmeli; personel seçildiyse hizmeti vermeli (süre personelin süresidir). Hizmetsiz bilette null.</summary>
    private async Task<Result<ServiceOffer?>> ResolveServiceAsync(Guid brandId, Guid branchId, Guid? serviceId, Guid? staffId, CancellationToken ct)
    {
        if (serviceId is not { } service)
        {
            if (staffId is { } onlyStaff && !(await tenants.GetStaffCardsAsync(brandId, [onlyStaff], ct)).ContainsKey(onlyStaff))
            {
                return Error.Validation("preferredStaffId", "invalid_format");
            }

            return (ServiceOffer?)null;
        }

        var resolved = await offers.ResolveAsync(brandId, branchId, [new ServiceOfferRequest(service, null), new ServiceOfferRequest(service, staffId)], ct);
        if (!resolved[0].IsSuccess)
        {
            return Error.Validation("serviceId", "invalid_format");
        }

        return resolved[1].IsSuccess ? resolved[1].Value : Error.Validation("preferredStaffId", "invalid_format");
    }

    private static Result<(Guid BranchId, string Name, PhoneNumber? Phone)> Validate(WalkInCreate body)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.BranchId is null)
        {
            errors["branchId"] = ["required"];
        }

        var name = body.FullName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["fullName"] = ["required"];
        }
        else if (name.Length is < NameMinLength or > NameMaxLength)
        {
            errors["fullName"] = ["out_of_range"];
        }

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(body.Phone) && !PhoneNumber.TryParse(body.Phone, out phone))
        {
            errors["phone"] = ["invalid_format"];
        }

        return errors.Count > 0 ? Error.Validation(errors) : (body.BranchId!.Value, name!, phone);
    }
}
