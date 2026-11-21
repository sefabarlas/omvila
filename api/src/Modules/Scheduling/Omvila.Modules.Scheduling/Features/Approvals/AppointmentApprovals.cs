using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>
/// "Onayla" (A2) ve "Reddet" (A4) (dilim-03 kabul kriterleri 2, 3, 5; sözleşme manageApproveAppointment,
/// manageDeclineAppointment). Kapsam dışı randevu 404, If-Match uymazsa 412, durum PendingApproval değilse 409
/// <c>invalid_state_transition</c>, tutma süresi geçmişse (iş henüz çalışmamış olsa da) 409 <c>hold_expired</c>. Dilim
/// yazılmadığı için kilit alınmaz; otomatik iptalle yarışta xmin denetimi ve işin <c>FOR UPDATE SKIP LOCKED</c> seçimi tam
/// olarak birinin kazanmasını sağlar (kaybeden onay 412 alır; T-63).
/// </summary>
internal sealed class AppointmentApprovals(SchedulingDbContext db, IOutbox outbox, IClock clock)
{
    // Kural: durum-makinesi.md A2
    public async Task<Result<Appointment>> ApproveAsync(Guid appointmentId, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        var appointment = await LoadAsync(appointmentId, access, expectedVersion, ct);
        if (!appointment.IsSuccess)
        {
            return appointment;
        }

        var approved = appointment.Value.Approve(clock.GetCurrentInstant(), userId);
        if (!approved.IsSuccess)
        {
            return approved.Error;
        }

        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(ApprovalEvents.Approved(appointment.Value, userId), ct);
        return appointment;
    }

    // Kural: durum-makinesi.md A4
    public async Task<Result<Appointment>> DeclineAsync(Guid appointmentId, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        var appointment = await LoadAsync(appointmentId, access, expectedVersion, ct);
        if (!appointment.IsSuccess)
        {
            return appointment;
        }

        var declined = appointment.Value.Decline(clock.GetCurrentInstant(), userId);
        if (!declined.IsSuccess)
        {
            return declined.Error;
        }

        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(ApprovalEvents.Declined(appointment.Value, userId), ct);
        return appointment;
    }

    private async Task<Result<Appointment>> LoadAsync(Guid appointmentId, ManageAccess access, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        var appointment = await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleOrDefaultAsync(a => a.Id == appointmentId, ct);
        if (appointment is null || !access.Covers(appointment))
        {
            return Error.NotFound();
        }

        return db.MatchesVersion(appointment, appointment.Version, expectedVersion) ? appointment : Error.PreconditionFailed();
    }
}
