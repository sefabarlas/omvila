using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// Kapıda sıra numarası (alan-modeli.md § WalkInTicket: BranchQueueCounter, "UPDATE … RETURNING"). Günün ilk bileti satırı
/// 1 ile açar, sonrakiler aynı satırı tek ifadede artırır: eşzamanlı istekler satır kilidinde sıralanır, aynı numara iki kez
/// verilmez. İsteğin transaction'ında çalışır; istek geri alınırsa numara da geri alınır.
/// </summary>
internal sealed class BranchQueueNumbers(SchedulingDbContext db)
{
    public async Task<int> NextAsync(Guid brandId, Guid branchId, LocalDate date, CancellationToken ct) =>
        (await db.Database.SqlQuery<int>($"""
            INSERT INTO scheduling.branch_queue_counters AS c (brand_id, branch_id, queue_date, last_number)
            VALUES ({brandId}, {branchId}, {date}, 1)
            ON CONFLICT (brand_id, branch_id, queue_date) DO UPDATE SET last_number = c.last_number + 1
            RETURNING c.last_number AS "Value"
            """).ToListAsync(ct)).Single();
}
