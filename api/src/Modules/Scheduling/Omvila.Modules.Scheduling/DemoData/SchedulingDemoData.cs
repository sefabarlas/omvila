using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-74) için randevu yazma yolu. Randevu <see cref="AppointmentWriter"/> ile
/// yazılır (kilit, çakışma, müsaitlik); durum yalnız aggregate metodlarıyla değişir. Geçmiş geçişler (Geldi, Gelmedi,
/// iptal) randevunun kendi zamanına göre kaydedilir. Entegrasyon olayı yazılmaz: demo kurulumu müşteriye SMS, hatırlatma
/// veya personel bildirimi üretmez. Çağıranın transaction'ında ve markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class SchedulingDemoData
{
    private readonly SchedulingDbContext _db;
    private readonly AppointmentWriter _writer;
    private readonly ITenantQueries _tenants;
    private readonly ICustomerDirectory _customers;

    internal SchedulingDemoData(SchedulingDbContext db, AppointmentWriter writer, ITenantQueries tenants, ICustomerDirectory customers)
    {
        _db = db;
        _writer = writer;
        _tenants = tenants;
        _customers = customers;
    }

    /// <summary>Randevuyu yazar ve istenen duruma getirir; yazılamazsa (ör. slot_taken) <see cref="DomainException"/>.</summary>
    /// <param name="input">Randevu.</param>
    /// <param name="now">Seed'in saati (IClock); iptal anı için.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<Guid> AddAsync(DemoAppointment input, Instant now, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var settings = await _tenants.GetAppointmentSettingsAsync(input.BrandId, ct)
            ?? throw new DomainException("Markanın randevu ayarları yok.");
        var online = input.Outcome == DemoAppointmentOutcome.PendingApproval;
        var slot = new SlotRequest(
            input.BranchId, input.StartUtc, [new RequestedItem(input.ServiceId, input.StaffId)], SameStaffForAll: false,
            online ? AvailabilityMode.Public : AvailabilityMode.Panel);
        var command = new CreateAppointmentCommand(
            input.BrandId, slot, input.CustomerId, input.BrandCustomerId,
            online ? AppointmentChannel.Online : AppointmentChannel.Phone,
            online ? ClientApp.BookingPage : ClientApp.Panel,
            CustomerNote: null, StaffNote: null, SeriesId: null, CreatedByUserId: null,
            Approval: online ? settings.Policy() : null);
        var written = await _writer.CreateAsync(command, ct);
        if (!written.IsSuccess)
        {
            throw new DomainException($"Demo randevusu yazılamadı: {written.Error.Code}");
        }

        var appointment = written.Value.Appointment;
        var noShowAfter = Duration.FromMinutes(settings.NoShowAfterMinutes);
        var changed = input.Outcome switch
        {
            DemoAppointmentOutcome.Confirmed => Expect(appointment, AppointmentStatus.Confirmed),
            DemoAppointmentOutcome.PendingApproval => Expect(appointment, AppointmentStatus.PendingApproval),
            DemoAppointmentOutcome.CheckedIn => appointment.CheckIn(input.StartUtc, userId: null),
            DemoAppointmentOutcome.NoShow => appointment.MarkNoShow(input.StartUtc + noShowAfter, noShowAfter),
            DemoAppointmentOutcome.CancelledBySalon => appointment.CancelBySalon(
                CancellationReason.CustomerRequested, note: null, userId: null, input.StartUtc > now ? now : input.StartUtc - Duration.FromHours(2)),
            DemoAppointmentOutcome.CancelledByCustomer => appointment.CancelByCustomer(
                CancellationReason.PlanChanged, note: null, now, Duration.FromHours(settings.CancellationCutoffHours)),
            _ => throw new ArgumentOutOfRangeException(nameof(input), input.Outcome, null),
        };
        if (!changed.IsSuccess)
        {
            throw new DomainException($"Demo randevusu {input.Outcome} durumuna geçemedi: {changed.Error.Code}");
        }

        await _db.SaveChangesAsync(ct);
        var counters = input.Outcome switch
        {
            DemoAppointmentOutcome.NoShow => new CustomerCounterChange(NoShows: 1),
            DemoAppointmentOutcome.CancelledByCustomer => new CustomerCounterChange(Cancels: 1),
            _ => null,
        };
        if (counters is not null && !(await _customers.ApplyCountersAsync(input.BrandId, input.BrandCustomerId, counters, ct)).IsSuccess)
        {
            throw new DomainException("Demo müşterisinin sayacı güncellenemedi.");
        }

        return appointment.Id;
    }

    private static Result Expect(Appointment appointment, AppointmentStatus status) =>
        appointment.Status == status ? Result.Success() : Error.Conflict(ErrorCodes.InvalidStateTransition);
}
