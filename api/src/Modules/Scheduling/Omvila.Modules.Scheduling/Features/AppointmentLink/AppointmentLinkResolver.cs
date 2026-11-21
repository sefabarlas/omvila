using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.PublicBooking;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Scheduling.Features.AppointmentLink;

/// <summary>
/// SMS bağlantısı (<c>/r/{code}</c>) kiracı bağlamı (ADR-0006 "erişim kodundan"; T-47 (9)). Kod tablosu marka RLS'lidir ve
/// isteğin markası henüz bilinmez: kod platform bağlamında aranır, oturumun önceki bağlamı geri yüklenir, kod geçerliyse
/// oturum yalnız kodun markasına kurulur (Tenancy SlugRegistry / <see cref="IPublicBookingQueries.EnterBranchAsync"/>
/// deseni). Biçimi bozuk, bilinmeyen, süresi dolmuş veya iptal edilmiş kod ayrımsız null döner (404 "Bu randevu
/// bulunamadı"); hangi kodun var olduğu dışarı sızmaz.
/// </summary>
internal sealed class AppointmentLinkResolver(
    SchedulingDbContext db,
    IDbSession session,
    IPublicBookingQueries booking,
    ITenantQueries tenants,
    IBrandBankAccounts bankAccounts,
    IClock clock)
{
    /// <summary>Randevuyu (kalemler ve dilimlerle, izlenen) yükler; oturum randevunun markasına geçer.</summary>
    // Kural: alan-modeli.md § AppointmentAccessCode (geçerlilik: iptal edilmemiş ve bitişten 7 gün)
    public async Task<Appointment?> EnterAsync(string? code, CancellationToken ct)
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
                .SingleOrDefaultAsync(c => c.Code == code && c.Purpose == AccessCodePurpose.Manage, ct);
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
        return await db.Appointments
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .SingleOrDefaultAsync(a => a.Id == accessCode.AppointmentId, ct);
    }

    /// <summary>Sözleşme PublicAppointment (<see cref="PublicAppointmentMapper"/>; oluşturma yanıtıyla aynı biçim). Marka bağlamında.</summary>
    public async Task<PublicAppointmentView?> MapAsync(Appointment appointment, string code, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        var card = await booking.GetBranchCardAsync(appointment.BrandId, appointment.BranchId, ct);
        var settings = await tenants.GetAppointmentSettingsAsync(appointment.BrandId, ct);
        if (card is null || settings is null)
        {
            return null;
        }

        var staffIds = appointment.Items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staff = await PublicAvailabilityEndpoints.PublicNamesAsync(tenants, appointment.BrandId, staffIds, ct);
        // V10: havale talimatı güncel aktif hesaptan; yalnız kapora bekleniyorsa okunur.
        var account = appointment.DisplayStatus == DisplayStatus.DepositPending ? await bankAccounts.GetActiveAsync(appointment.BrandId, ct) : null;
        return PublicAppointmentMapper.Map(appointment, code, booking.AppointmentPageUrl(code), card, settings, staff, account, clock.GetCurrentInstant());
    }
}
