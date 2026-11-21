using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// Süresi dolmuş tutmaların (A5) satır kilidiyle seçimi. <c>FOR UPDATE SKIP LOCKED</c>: o an başka bir transaction'ın
/// değiştirdiği satır (ör. aynı anda "Onayla") atlanır; kilitlenen satır transaction sonuna kadar bizimdir, eşzamanlı onay
/// UPDATE'i bekler ve xmin uymadığı için 412 alır. Böylece onay ile otomatik iptal yarışında tam olarak biri kazanır
/// (T-63). Kiracı bağlamında (RLS) çalışır.
/// </summary>
internal static class ExpiredHolds
{
    /// <summary>Verilen randevulardan hâlâ onay bekleyen ve süresi dolmuş olanları kilitler.</summary>
    public static async Task<List<Guid>> LockAsync(SchedulingDbContext db, IReadOnlyCollection<Guid> appointmentIds, Instant now, CancellationToken ct)
    {
        var ids = appointmentIds.ToArray();
        return await db.Database.SqlQuery<Guid>(
                $"""
                SELECT id AS "Value" FROM scheduling.appointments
                WHERE id = ANY({ids}) AND status = 'PendingApproval' AND hold_expires_utc <= {now}
                ORDER BY id
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(ct);
    }

    /// <summary>Markanın süresi dolmuş bütün tutmalarını kilitler (ExpireAppointmentHolds).</summary>
    public static async Task<List<Guid>> LockForBrandAsync(SchedulingDbContext db, Guid brandId, Instant now, CancellationToken ct) =>
        await db.Database.SqlQuery<Guid>(
                $"""
                SELECT id AS "Value" FROM scheduling.appointments
                WHERE brand_id = {brandId} AND status = 'PendingApproval' AND hold_expires_utc <= {now}
                ORDER BY id
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(ct);
}
