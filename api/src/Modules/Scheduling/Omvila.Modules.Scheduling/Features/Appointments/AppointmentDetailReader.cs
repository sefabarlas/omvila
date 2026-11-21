using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// Randevu yan paneli (sözleşme AppointmentDetail). Müşteri Customers'tan, personel adı ve şube saat dilimi Tenancy'den
/// toplu okunur. Geçmiş (history) ayrı tabloda tutulmaz; randevunun zaman damgalarından ve saat değişikliğinde işaretlenen
/// dilimlerden türetilir (T-50).
/// </summary>
internal sealed class AppointmentDetailReader(
    SchedulingDbContext db, ITenantQueries tenants, ICustomerDirectory customers, IAppointmentPayments payments, IClock clock)
{
    /// <summary>Randevu (kalemler ve dilimlerle, izlenen). Başka marka RLS ile görünmez.</summary>
    public Task<Appointment?> LoadAsync(Guid appointmentId, CancellationToken ct) =>
        db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleOrDefaultAsync(a => a.Id == appointmentId, ct);

    public async Task<AppointmentDetail> MapAsync(Guid brandId, Appointment appointment, ManageAccess access, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        ArgumentNullException.ThrowIfNull(access);
        var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, appointment.BranchId, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var settings = await tenants.GetAppointmentSettingsAsync(brandId, ct) ?? throw new DomainException("Marka ayarları bulunamadı.");
        var info = (await customers.GetDisplayInfoAsync(brandId, [appointment.BrandCustomerId], ct)).GetValueOrDefault(appointment.BrandCustomerId);
        var staffIds = appointment.Items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var today = clock.GetCurrentInstant().InZone(zone).Date;
        return new AppointmentDetail(
            appointment.Id,
            appointment.BranchId,
            appointment.Status.ToString(),
            appointment.DisplayStatus.ToString(),
            appointment.Channel.ToString(),
            appointment.StartUtc,
            appointment.EndUtc,
            zone.Id,
            appointment.HoldExpiresUtc,
            Customer(appointment, info, access.HasPhoneView || !settings.MaskCustomerPhoneFromStaff, today),
            [.. appointment.Items.OrderBy(i => i.SeqNo).Select(i => Item(i, staff, currency))],
            DepositView.For(appointment, currency),
            new Money(appointment.TotalAmountMinor, currency),
            // "Salonda ödenecek" = toplam − salonda kalan kapora (kaporasız onayda toplam; tamamlanmışta 0).
            new Money(appointment.AmountDueMinor, currency),
            appointment.CustomerNote,
            appointment.StaffNote,
            appointment.SeriesId is not null,
            appointment.SeriesId,
            // "Gelmezse 13:15'te otomatik Gelmedi olur": yalnız onaylı ve gelmemiş randevuda.
            appointment.Status == AppointmentStatus.Confirmed ? appointment.StartUtc + Duration.FromMinutes(settings.NoShowAfterMinutes) : null,
            Cancellation(appointment),
            appointment.CompletedUtc,
            access.HasPaymentView ? await PaymentsAsync(brandId, appointment, staff, ct) : null,
            History(appointment),
            appointment.Version.ToString(CultureInfo.InvariantCulture));
    }

    private async Task<IReadOnlyList<PaymentView>> PaymentsAsync(
        Guid brandId, Appointment appointment, IReadOnlyDictionary<Guid, StaffCard> staff, CancellationToken ct)
    {
        var records = await payments.ListAsync(brandId, appointment.Id, ct);
        if (records.Count == 0)
        {
            return [];
        }

        var items = appointment.Items.Where(i => i.Status != AppointmentItemStatus.Cancelled).OrderBy(i => i.SeqNo).ToList();
        var serviceNames = items.Select(i => i.ServiceNameSnapshot).ToList();
        var staffName = items.Select(i => i.StaffId).FirstOrDefault(s => s is not null) is { } first && staff.TryGetValue(first, out var card)
            ? card.FullName
            : null;
        var hadDeposit = records.Any(r => r.IsDeposit);
        return [.. records.Select(r => new PaymentView(
            r.Id,
            r.BranchId,
            r.Method,
            r.Amount,
            r.IsDeposit,
            r.Status,
            r.RefundedAmountMinor > 0 ? new Money(r.RefundedAmountMinor, r.Amount.Currency) : null,
            r.PaidUtc,
            null,
            r.Reference,
            "Appointment",
            appointment.Id,
            appointment.StartUtc,
            serviceNames,
            hadDeposit && !r.IsDeposit,
            staffName))];
    }

    private static AppointmentCustomerView Customer(Appointment appointment, CustomerDisplayInfo? info, bool canSeeFullPhone, LocalDate today)
    {
        if (info is null)
        {
            return new AppointmentCustomerView(appointment.BrandCustomerId, string.Empty, null, null, 0, null, false);
        }

        var phone = canSeeFullPhone && PhoneNumber.TryParse(info.PhoneE164, out var parsed) ? parsed.E164 : null;
        return new AppointmentCustomerView(info.BrandCustomerId, info.FullName, phone, info.PhoneMasked, info.VisitCount, info.LastVisitOn, info.IsNewOn(today));
    }

    private static AppointmentItemView Item(AppointmentItem item, IReadOnlyDictionary<Guid, StaffCard> staff, Currency currency) => new(
        item.Id,
        item.ServiceId,
        item.ServiceNameSnapshot,
        item.StaffId,
        item.StaffId is { } staffId && staff.TryGetValue(staffId, out var card) ? card.FullName : null,
        item.StartUtc,
        item.EndUtc,
        new Money(item.PriceMinor, currency),
        new Money(item.StaffSurchargeMinor, currency),
        new Money(item.DiscountMinor, currency),
        item.CustomerPackageItemId is not null,
        item.Status.ToString(),
        [.. item.CurrentSegments.Select(s => new SegmentView(s.During.Start, s.During.End, s.StaffBusy))]);

    private static AppointmentCancellationView? Cancellation(Appointment a) =>
        a is { Status: AppointmentStatus.Cancelled, CancelledUtc: { } at, CancelledBy: { } by, CancellationReason: { } reason }
            ? new AppointmentCancellationView(at, by.ToString(), reason.ToString(), a.CancellationNote)
            : null;

    /// <summary>
    /// Zaman damgalarından geçmiş: oluşturma, havale bildirimi, onay (talep oluşturulduktan sonra onaylandıysa; A13 sonrası
    /// yeniden onayda yalnız son onay görünür), "Kapora geldi" (onay yerine), "Kaporasız onayla", her saat değişikliği (eski
    /// dilimlerin işaretlendiği an), gelmedi, geldi (gelmedi sonrasındaysa "Geldiyse geri al"), tamamlama, iptal (salon reddi
    /// Declined, süresi dolan tutma HoldExpired), kapora hatırlatması ve "Salon iade etti" (not detail'de; geçmiş yalnız
    /// panelde döner). Yapan kişinin adı tutulmadığından actorName yazılmaz.
    /// </summary>
    internal static IReadOnlyList<AppointmentHistoryEntry> History(Appointment a)
    {
        var entries = new List<(Instant At, int Order, string Kind)> { (a.CreatedUtc, 0, "Created") };
        if (a.DepositCustomerReportedUtc is { } reported)
        {
            entries.Add((reported, 1, "DepositReported"));
        }

        if (a.DepositRemindedUtc is { } reminded)
        {
            entries.Add((reminded, 1, "DepositReminded"));
        }

        // "Kapora geldi" onay sayılır (A3): aynı anda ayrıca Approved yazılmaz. Kaporasız onayda Approved + DepositWaived.
        if (a.ConfirmedUtc is { } confirmed && confirmed > a.CreatedUtc && confirmed != a.DepositReceivedUtc)
        {
            entries.Add((confirmed, 1, "Approved"));
        }

        if (a.DepositReceivedUtc is { } received)
        {
            entries.Add((received, 1, "DepositReceived"));
        }

        if (a.DepositWaivedUtc is { } waived)
        {
            entries.Add((waived, 2, "DepositWaived"));
        }

        entries.AddRange(a.Items.SelectMany(i => i.Segments).Where(s => s.SupersededUtc is not null)
            .Select(s => s.SupersededUtc!.Value).Distinct().Select(at => (at, 1, "Rescheduled")));
        if (a.NoShowUtc is { } noShow)
        {
            entries.Add((noShow, 2, "NoShow"));
        }

        if (a.CheckedInUtc is { } checkedIn)
        {
            entries.Add((checkedIn, 3, a.NoShowUtc is { } ns && checkedIn > ns ? "NoShowUndone" : "CheckedIn"));
        }

        if (a.CompletedUtc is { } completed)
        {
            entries.Add((completed, 4, "Completed"));
        }

        if (a.CancelledUtc is { } cancelled)
        {
            entries.Add((cancelled, 5, CancelKind(a)));
        }

        if (a.DepositRefundedUtc is { } refunded)
        {
            entries.Add((refunded, 6, "DepositRefunded"));
        }

        return [.. entries.OrderBy(e => e.At).ThenBy(e => e.Order)
            .Select(e => new AppointmentHistoryEntry(e.At, e.Kind, null, string.Equals(e.Kind, "DepositRefunded", StringComparison.Ordinal) ? a.DepositRefundNote : null))];
    }

    /// <summary>Sözleşme AppointmentHistoryEntry.kind: A4 Declined, A5 HoldExpired, diğer iptaller Cancelled.</summary>
    private static string CancelKind(Appointment a) => a.CancellationReason switch
    {
        CancellationReason.DeclinedBySalon => "Declined",
        CancellationReason.ApprovalExpired or CancellationReason.DepositNotReceived when a.CancelledBy == CancelledBy.System => "HoldExpired",
        _ => "Cancelled",
    };
}
