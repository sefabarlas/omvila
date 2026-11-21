using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// "Yeni randevu" (sözleşme manageCreateAppointment; dilim-02 kabul kriterleri 11 ve 13). Randevu yalnız
/// <see cref="AppointmentWriter"/> ile yazılır. Tekrarlayan randevuda her tekrar ayrı yazma turudur (T-49): çakışan, çalışma
/// saati dışı veya izinli (onaysız) tekrar atlanır ve <c>skippedOccurrences</c>'ta döner; kilit zaman aşımı bütün isteği
/// 503 ile geri alır. Yan etki: onaylı her randevu için <see cref="AppointmentConfirmed"/> outbox olayı (bildirim PR-9).
/// </summary>
internal sealed class ManageAppointmentCreator(
    SchedulingDbContext db,
    AppointmentWriter writer,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    ISubscriptionQueries subscriptions,
    IOutbox outbox,
    IIdGenerator ids,
    IClock clock)
{
    private const int NameMinLength = 2;
    private const int NameMaxLength = 120;
    private const int StaffNoteMaxLength = 1000;
    private const string ReadOnlyStatus = "ReadOnly";
    private const int AlternativeCount = 3;

    // Kural: durum-makinesi.md A1, musaitlik.md § Panelden elle ekleme, dilim-02 § Alan kuralları (tekrarlayan randevu)
    public async Task<Result<(Appointment Appointment, IReadOnlyList<LocalDate> Skipped)>> CreateAsync(
        Guid brandId, ManageAppointmentCreate body, ManageAccess access, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(access);
        var prepared = await PrepareAsync(brandId, body, access, userId, ct);
        if (!prepared.IsSuccess)
        {
            return prepared.Error;
        }

        var (command, zone, recurrence) = prepared.Value;
        var notify = body.NotifyCustomer ?? true;
        var first = await writer.CreateAsync(command, ct);
        if (!first.IsSuccess)
        {
            return await WithAlternativesAsync(first.Error, brandId, command.Slot, command.CustomerId, ct);
        }

        var appointment = first.Value.Appointment;
        if (recurrence is null)
        {
            await PublishAsync(appointment, notify, isSeriesStart: false, ct);
            return (appointment, []);
        }

        var series = await StartSeriesAsync(appointment, zone, recurrence.Rule, recurrence.EndsOn, ct);
        await PublishAsync(appointment, notify, isSeriesStart: true, ct);
        var skipped = await CreateOccurrencesAsync(command, series, recurrence, zone, notify, ct);
        return skipped.IsSuccess ? (appointment, skipped.Value) : skipped.Error;
    }

    /// <summary>Kapsam (404), salt okunur abonelik (403), biçim (400), personel K (403) ve müşteri çözümü; yazma komutu.</summary>
    private async Task<Result<(CreateAppointmentCommand Command, DateTimeZone Zone, ValidRecurrence? Recurrence)>> PrepareAsync(
        Guid brandId, ManageAppointmentCreate body, ManageAccess access, Guid? userId, CancellationToken ct)
    {
        if (body.BranchId is not { } branchId)
        {
            return Error.Validation("branchId", "required");
        }

        if (!access.Branches.Includes(branchId) || await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) is not { } zoneId)
        {
            return Error.NotFound();
        }

        // Kural: durum-makinesi.md § Abonelik ReadOnly ("yeni randevu ... kapalı"), acik-sorular.md S-05 (tasarım: hayır).
        if (await subscriptions.GetBannerAsync(brandId, ct) is { Status: ReadOnlyStatus })
        {
            return Error.Forbidden(ErrorCodes.SubscriptionReadOnly);
        }

        var zone = DateTimeZoneProviders.Tzdb[zoneId];
        var input = Validate(body, zone);
        if (!input.IsSuccess)
        {
            return input.Error;
        }

        var (channel, start, items, recurrence, newCustomerPhone) = input.Value;

        // Personel (K) yalnız kendine randevu ekler (roller-yetkiler.md appointment.create "K (kendine)").
        if (items.Any(i => !access.IsOwnStaff(i.StaffId)))
        {
            return Error.Forbidden();
        }

        var customer = await ResolveCustomerAsync(brandId, body, channel, newCustomerPhone, ct);
        if (!customer.IsSuccess)
        {
            return customer.Error;
        }

        var slot = new SlotRequest(branchId, start, items, SameStaffForAll: false, AvailabilityMode.Panel, body.AllowOutsideWorkingHours ?? false);
        var command = new CreateAppointmentCommand(
            brandId, slot, customer.Value.CustomerId, customer.Value.BrandCustomerId, channel, ClientApp.Panel, CustomerNote: null, body.StaffNote, SeriesId: null, userId);
        return (command, zone, recurrence);
    }

    /// <summary>
    /// Serinin sonraki tarihleri (ilk tarih hariç, ufuk 3 ay). Çakışan (slot_taken), saat dışı veya izinli (onaysız) tekrar
    /// atlanır; ilk randevu kullanıcının seçtiği saattir ve atlanmaz (hatası doğrudan döner), bu yüzden "hepsi atlandı"
    /// durumu oluşmaz (T-50). Kilit zaman aşımı (503) bütün isteği geri alır.
    /// </summary>
    private async Task<Result<IReadOnlyList<LocalDate>>> CreateOccurrencesAsync(
        CreateAppointmentCommand command, AppointmentSeries series, ValidRecurrence recurrence, DateTimeZone zone, bool notify, CancellationToken ct)
    {
        var skipped = new List<LocalDate>();
        var dates = RecurrenceGenerator.Dates(series.StartsOn, recurrence.Rule, RecurrenceGenerator.HorizonEnd(series.StartsOn), recurrence.EndsOn).Skip(1);
        foreach (var date in dates)
        {
            var slot = command.Slot with { StartUtc = RecurrenceGenerator.StartOf(date, series.LocalStartTime, zone) };
            var occurrence = await writer.CreateAsync(command with { Slot = slot, SeriesId = series.Id }, ct);
            if (occurrence.IsSuccess)
            {
                await PublishAsync(occurrence.Value.Appointment, notify, isSeriesStart: false, ct);
            }
            else if (occurrence.Error.Kind == ErrorKind.Unavailable)
            {
                return occurrence.Error;
            }
            else
            {
                skipped.Add(date);
            }
        }

        return skipped;
    }

    /// <summary>
    /// Seri şablonu ilk randevunun planından çıkar (kalem, personel, ilk başlangıca göre dakika); ufuk ilk tarihten 3 ay
    /// (hariç) veya bitiş tarihi (T-47).
    /// </summary>
    private async Task<AppointmentSeries> StartSeriesAsync(Appointment first, DateTimeZone zone, WeeklyRecurrence rule, LocalDate? endsOn, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var local = first.StartUtc.InZone(zone);
        var templates = first.Items.OrderBy(i => i.SeqNo)
            .Select(i => new SeriesItemTemplate(i.ServiceId, i.StaffId, (int)(i.StartUtc - first.StartUtc).TotalMinutes))
            .ToList();
        var lastConsidered = RecurrenceGenerator.HorizonEnd(local.Date).PlusDays(-1);
        var generatedUntil = endsOn is { } end && end < lastConsidered ? end : lastConsidered;
        var series = AppointmentSeries.Create(
            ids.New(), first.BrandId, first.BranchId, first.CustomerId, first.BrandCustomerId, templates, rule, local.Date, local.TimeOfDay, endsOn, generatedUntil, now);
        db.AppointmentSeries.Add(series);
        first.AssignSeries(series.Id, now);
        await db.SaveChangesAsync(ct);
        return series;
    }

    private async Task PublishAsync(Appointment appointment, bool notify, bool isSeriesStart, CancellationToken ct)
    {
        // Kapıda sıra Geldi olarak başlar; onay mesajı yalnız onaylı randevuda (bildirimler.md appointment.confirmed).
        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return;
        }

        await outbox.EnqueueAsync(
            new AppointmentConfirmed(
                appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc,
                appointment.Channel.ToString(), notify, appointment.SeriesId, isSeriesStart),
            ct);
    }

    private async Task<Error> WithAlternativesAsync(Error error, Guid brandId, SlotRequest slot, Guid customerId, CancellationToken ct)
    {
        if (!string.Equals(error.Code, ErrorCodes.SlotTaken, StringComparison.Ordinal))
        {
            return error;
        }

        var alternatives = await writer.SuggestAlternativesAsync(brandId, slot, AlternativeCount, customerId, ignoreAppointmentId: null, ct);
        return Error.Conflict(ErrorCodes.SlotTaken, AppointmentWriter.AlternativesMeta(alternatives));
    }

    private async Task<Result<(Guid CustomerId, Guid BrandCustomerId)>> ResolveCustomerAsync(
        Guid brandId, ManageAppointmentCreate body, AppointmentChannel channel, PhoneNumber? newCustomerPhone, CancellationToken ct)
    {
        if (body.BrandCustomerId is { } brandCustomerId)
        {
            var info = await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct);
            return info.TryGetValue(brandCustomerId, out var card) ? (card.CustomerId, card.BrandCustomerId) : Error.NotFound();
        }

        var source = channel switch
        {
            AppointmentChannel.Phone => CustomerSource.Phone,
            AppointmentChannel.WalkIn => CustomerSource.WalkIn,
            _ => CustomerSource.Panel,
        };
        var ensured = await customers.EnsureBrandCustomerAsync(
            new EnsureBrandCustomerRequest(brandId, newCustomerPhone!, body.NewCustomer!.FullName!.Trim(), source, PhoneVerified: false), ct);
        return (ensured.CustomerId, ensured.BrandCustomerId);
    }

    /// <summary>Biçim denetimi (sözleşme ManageAppointmentCreate).</summary>
    private static Result<ValidInput> Validate(ManageAppointmentCreate body, DateTimeZone zone)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        AppointmentChannel channel = default;
        if (body.Channel is null)
        {
            errors["channel"] = ["required"];
        }
        else if (!Enum.TryParse(body.Channel, ignoreCase: false, out channel) || channel is not (AppointmentChannel.Panel or AppointmentChannel.Phone or AppointmentChannel.WalkIn))
        {
            errors["channel"] = ["invalid_format"];
        }

        if (body.StartUtc is null)
        {
            errors["startUtc"] = ["required"];
        }

        if (body.StaffNote is { Length: > StaffNoteMaxLength })
        {
            errors["staffNote"] = ["too_long"];
        }

        var items = ValidateItems(body.Items, errors);
        var phone = ValidateCustomer(body, errors);
        var recurrence = ValidateRecurrence(body.Recurrence, channel, body.StartUtc?.InZone(zone).Date, errors);
        return errors.Count > 0 ? Error.Validation(errors) : new ValidInput(channel, body.StartUtc!.Value, items, recurrence, phone);
    }

    private static List<RequestedItem> ValidateItems(IReadOnlyList<AppointmentItemRequest>? requested, Dictionary<string, string[]> errors)
    {
        var items = new List<RequestedItem>();
        if (requested is null || requested.Count == 0)
        {
            errors["items"] = ["required"];
            return items;
        }

        if (requested.Count > PlatformDefaults.MaxItemsPerAppointment)
        {
            errors["items"] = ["out_of_range"];
            return items;
        }

        for (var i = 0; i < requested.Count; i++)
        {
            var item = requested[i];
            if (item?.ServiceId is null)
            {
                errors[$"items[{i}].serviceId"] = ["required"];
            }

            // Panelde personel her kalemde zorunludur (sözleşme: items[].staffId required).
            if (item?.StaffId is null)
            {
                errors[$"items[{i}].staffId"] = ["required"];
            }

            if (item?.ServiceId is { } serviceId && item.StaffId is { } staffId)
            {
                items.Add(new RequestedItem(serviceId, staffId));
            }
        }

        return items;
    }

    /// <summary>brandCustomerId ile newCustomer'dan tam biri (XOR).</summary>
    private static PhoneNumber? ValidateCustomer(ManageAppointmentCreate body, Dictionary<string, string[]> errors)
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

    private static ValidRecurrence? ValidateRecurrence(
        RecurrenceRequest? recurrence, AppointmentChannel channel, LocalDate? firstDate, Dictionary<string, string[]> errors)
    {
        if (recurrence is null)
        {
            return null;
        }

        if (!string.Equals(recurrence.Frequency, "Weekly", StringComparison.Ordinal))
        {
            errors["recurrence.frequency"] = [recurrence.Frequency is null ? "required" : "invalid_format"];
        }

        // Kapıda sıra o anki ziyarettir; tekrarlayan olamaz.
        if (channel == AppointmentChannel.WalkIn)
        {
            errors["recurrence"] = ["invalid_format"];
        }

        if (recurrence.EndsOn is { } endsOn && firstDate is { } first && endsOn < first)
        {
            errors["recurrence.endsOn"] = ["out_of_range"];
        }

        var rule = WeeklyRecurrence.Create(recurrence.IntervalWeeks ?? 0);
        if (!rule.IsSuccess)
        {
            errors["recurrence.intervalWeeks"] = [recurrence.IntervalWeeks is null ? "required" : "invalid_format"];
            return null;
        }

        return new ValidRecurrence(rule.Value, recurrence.EndsOn);
    }

    private sealed record ValidRecurrence(WeeklyRecurrence Rule, LocalDate? EndsOn);

    private sealed record ValidInput(
        AppointmentChannel Channel, Instant Start, IReadOnlyList<RequestedItem> Items, ValidRecurrence? Recurrence, PhoneNumber? NewCustomerPhone);
}
