using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Jobs;

/// <summary>
/// İşi bir kez çalıştırır: pg_try_advisory_lock ile birden çok worker örneğinde yalnız biri çalışır,
/// sonuç infra.job_runs'a yazılır (ADR-0005).
/// </summary>
public sealed partial class JobRunner(
    IServiceScopeFactory scopes, NpgsqlDataSource dataSource, IClock clock, IIdGenerator ids, ILogger<JobRunner> logger)
{
    public async Task<JobOutcome> RunOnceAsync(Type jobType, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(jobType);
        await using var scope = scopes.CreateAsyncScope();
        var job = (IPeriodicJob)scope.ServiceProvider.GetRequiredService(jobType);

        // Kilit, işin transaction'ından bağımsız ayrı bir bağlantıda tutulur (oturum düzeyinde).
        await using var lockConnection = await dataSource.OpenConnectionAsync(ct);
        if (!await AdvisoryLockAsync(lockConnection, "pg_try_advisory_lock", job.Key, ct))
        {
            LogSkipped(logger, job.Key);
            return JobOutcome.Skipped;
        }

        var started = clock.GetCurrentInstant();
        var outcome = JobOutcome.Succeeded;
        string? error = null;
        try
        {
            await job.RunAsync(ct);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            outcome = JobOutcome.Failed;
            error = exception.GetType().Name;
            LogFailed(logger, exception, job.Key);
        }
        finally
        {
            await RecordAsync(job.Key, started, outcome, error, CancellationToken.None);
            await AdvisoryLockAsync(lockConnection, "pg_advisory_unlock", job.Key, CancellationToken.None);
        }

        return outcome;
    }

    private static async Task<bool> AdvisoryLockAsync(NpgsqlConnection connection, string function, string key, CancellationToken ct)
    {
        await using var command = new NpgsqlCommand($"SELECT {function}(hashtextextended($1, 0))", connection);
        command.Parameters.Add(new NpgsqlParameter { Value = key });
        return (bool)(await command.ExecuteScalarAsync(ct))!;
    }

    private async Task RecordAsync(string key, Instant started, JobOutcome outcome, string? error, CancellationToken ct)
    {
        await using var scope = scopes.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(ct);
        var db = scope.ServiceProvider.GetRequiredService<InfraDbContext>();
        db.JobRuns.Add(new JobRun
        {
            Id = ids.New(),
            JobKey = key,
            StartedUtc = started,
            FinishedUtc = clock.GetCurrentInstant(),
            Status = outcome.ToString(),
            Error = error,
        });
        await db.SaveChangesAsync(ct);
        await session.CommitAsync(ct);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Job {JobKey} skipped: another instance holds the lock")]
    private static partial void LogSkipped(ILogger logger, string jobKey);

    [LoggerMessage(Level = LogLevel.Error, Message = "Job {JobKey} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, string jobKey);
}
