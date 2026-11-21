using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// <see cref="IAppointmentReviewLinks"/>: /d/ kodu (AppointmentAccessCode, Purpose Review) ve değerlendirilecek randevu. Kod
/// çözümü /r/ bağlantısıyla aynı desendir (AppointmentLinkResolver): kod platform bağlamında aranır, önceki bağlam geri yüklenir,
/// geçerliyse oturum kodun markasına kurulur.
/// </summary>
internal sealed class AppointmentReviewLinks(SchedulingDbContext db, IDbSession session, IIdGenerator ids, IClock clock) : IAppointmentReviewLinks
{
    public async Task<AppointmentReviewSubject?> GetSubjectAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        var appointment = await db.Appointments.AsNoTracking().Include(a => a.Items)
            .SingleOrDefaultAsync(a => a.BrandId == brandId && a.Id == appointmentId, ct);
        return appointment is null ? null : Map(appointment);
    }

    // Kural: alan-modeli.md § AppointmentAccessCode (Purpose Review, /d/)
    public async Task<string?> IssueReviewCodeAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var appointment = await db.Appointments.AsNoTracking()
            .Where(a => a.BrandId == brandId && a.Id == appointmentId)
            .Select(a => new { a.Id, a.EndUtc })
            .SingleOrDefaultAsync(ct);
        if (appointment is null)
        {
            return null;
        }

        var existing = await db.AppointmentAccessCodes.AsNoTracking()
            .Where(c => c.BrandId == brandId && c.AppointmentId == appointmentId && c.Purpose == AccessCodePurpose.Review
                && c.RevokedUtc == null && c.ExpiresUtc > now)
            .OrderByDescending(c => c.CreatedUtc)
            .Select(c => c.Code)
            .FirstOrDefaultAsync(ct);
        if (existing is not null)
        {
            return existing;
        }

        var code = AppointmentAccessCode.IssueForReview(ids.New(), brandId, appointmentId, appointment.EndUtc, now);
        db.AppointmentAccessCodes.Add(code);
        await db.SaveChangesAsync(ct);
        return code.Code;
    }

    // Kural: alan-modeli.md § AppointmentAccessCode (geçerlilik: iptal edilmemiş ve süresi dolmamış)
    public async Task<AppointmentReviewSubject?> EnterByReviewCodeAsync(string? code, CancellationToken ct)
    {
        if (!AccessCodeGenerator.IsWellFormed(code))
        {
            return null;
        }

        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        AppointmentAccessCode? accessCode;
        try
        {
            accessCode = await db.AppointmentAccessCodes.AsNoTracking()
                .SingleOrDefaultAsync(c => c.Code == code && c.Purpose == AccessCodePurpose.Review, ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }

        if (accessCode is null || !accessCode.IsValidAt(clock.GetCurrentInstant()))
        {
            return null;
        }

        await session.SetTenantAsync(TenantScope.ForBrand(accessCode.BrandId), ct);
        return await GetSubjectAsync(accessCode.BrandId, accessCode.AppointmentId, ct);
    }

    private static AppointmentReviewSubject Map(Appointment appointment)
    {
        var ordered = appointment.Items.OrderBy(i => i.SeqNo).ToList();
        return new AppointmentReviewSubject(
            appointment.BrandId,
            appointment.Id,
            appointment.BranchId,
            appointment.BrandCustomerId,
            appointment.Status.ToString(),
            appointment.StartUtc,
            [.. ordered.Select(i => i.ServiceNameSnapshot)],
            ordered.FirstOrDefault(i => i.StaffId is not null)?.StaffId);
    }
}
