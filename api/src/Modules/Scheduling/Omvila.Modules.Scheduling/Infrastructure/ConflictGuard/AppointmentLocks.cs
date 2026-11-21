using NodaTime;
using NodaTime.Text;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;

/// <summary>
/// Çakışma engeli katman 2 (musaitlik.md § Katman 2, ADR-0004): isteğin transaction'ında
/// <c>pg_advisory_xact_lock(hashtext('staff:' || staff_id || ':' || local_date))</c>, sonra kaynaklar için
/// <c>'resource:' || resource_id || ':' || local_date</c>. Kilitler her zaman aynı sırada alınır (önce personel, sonra
/// kaynak; grup içinde kimlik, sonra gün) ki iki yazma birbirini beklerken kilitlenmesin (C7). Kilitler commit veya
/// rollback'te kendiliğinden bırakılır.
/// </summary>
internal sealed class AppointmentLocks(IDbSession session)
{
    private const string Savepoint = "omvila_appointment_locks";

    /// <summary>Personel + şubenin yerel günü anahtarı (uuid PostgreSQL metin biçiminde, gün ISO).</summary>
    public static string StaffKey(Guid staffId, LocalDate date) => $"staff:{staffId:D}:{LocalDatePattern.Iso.Format(date)}";

    public static string ResourceKey(Guid resourceId, LocalDate date) => $"resource:{resourceId:D}:{LocalDatePattern.Iso.Format(date)}";

    /// <summary>Kilit anahtarları alınacakları sırayla: personeller, sonra kaynaklar; her grupta kimlik, sonra gün.</summary>
    public static IReadOnlyList<string> OrderedKeys(IEnumerable<Guid> staffIds, IEnumerable<Guid> resourceIds, IEnumerable<LocalDate> dates)
    {
        var days = dates.Distinct().Order().ToList();
        var staff = staffIds.Distinct().Order().SelectMany(id => days.Select(d => StaffKey(id, d)));
        var resources = resourceIds.Distinct().Order().SelectMany(id => days.Select(d => ResourceKey(id, d)));
        return [.. staff.Concat(resources)];
    }

    /// <summary>
    /// Kilitleri sırayla alır. <c>lock_timeout</c> (musaitlik.md: 5 sn) aşılırsa veya PostgreSQL kilitlenme saptarsa
    /// 503 <c>try_again</c> döner; kilit denemesi bir savepoint içinde olduğu için transaction kullanılabilir kalır.
    /// <c>lock_timeout</c> transaction sonuna kadar geçerlidir (SET LOCAL); sonraki satır kilitlerini de sınırlar.
    /// </summary>
    public async Task<Result> AcquireAsync(IReadOnlyList<string> orderedKeys, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(orderedKeys);
        if (orderedKeys.Count == 0)
        {
            return Result.Success();
        }

        var transaction = session.Transaction;
        await transaction.SaveAsync(Savepoint, ct);
        try
        {
            await using var batch = new NpgsqlBatch(session.Connection, transaction);
            batch.BatchCommands.Add(new NpgsqlBatchCommand("SELECT set_config('lock_timeout', $1, true)")
            {
                Parameters = { new NpgsqlParameter { Value = $"{PlatformDefaults.AppointmentLockTimeoutSeconds}s" } },
            });
            foreach (var key in orderedKeys)
            {
                batch.BatchCommands.Add(new NpgsqlBatchCommand("SELECT pg_advisory_xact_lock(hashtext($1))")
                {
                    Parameters = { new NpgsqlParameter { Value = key } },
                });
            }

            await batch.ExecuteNonQueryAsync(ct);
        }
        catch (PostgresException ex) when (ex.SqlState is PostgresErrorCodes.LockNotAvailable or PostgresErrorCodes.DeadlockDetected)
        {
            await transaction.RollbackAsync(Savepoint, ct);
            return Error.Unavailable();
        }

        await transaction.ReleaseAsync(Savepoint, ct);
        return Result.Success();
    }
}
