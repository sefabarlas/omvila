using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: ADR-0005 (pg_try_advisory_lock ile tek örnek; çalışma kaydı infra.job_runs)
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class JobRunnerTests(PostgresFixture db) : IAsyncLifetime
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync() => await InfraTables.ResetAsync(db, Ct);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task SucceedingJob_IsRecorded()
    {
        await using var services = Services();

        (await Runner(services).RunOnceAsync(typeof(SucceedingJob), Ct)).ShouldBe(JobOutcome.Succeeded);

        (await InfraTables.ScalarAsync<string>(db, "SELECT status FROM infra.job_runs WHERE job_key = 'TestSucceedingJob'", Ct))
            .ShouldBe("Succeeded");
    }

    [Fact]
    public async Task FailingJob_IsRecordedWithErrorTypeOnly()
    {
        await using var services = Services();

        (await Runner(services).RunOnceAsync(typeof(FailingJob), Ct)).ShouldBe(JobOutcome.Failed);

        (await InfraTables.ScalarAsync<string>(db, "SELECT status || ':' || error FROM infra.job_runs WHERE job_key = 'TestFailingJob'", Ct))
            .ShouldBe("Failed:InvalidOperationException");
    }

    [Fact]
    public async Task ConcurrentRun_OfSameJob_IsSkipped()
    {
        var gate = new Gate();
        await using var services = Services(gate);
        var runner = Runner(services);

        var first = runner.RunOnceAsync(typeof(BlockingJob), Ct);
        await gate.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10), Ct);

        (await runner.RunOnceAsync(typeof(BlockingJob), Ct)).ShouldBe(JobOutcome.Skipped);

        gate.Release.SetResult();
        (await first).ShouldBe(JobOutcome.Succeeded);
        (await runner.RunOnceAsync(typeof(BlockingJob), Ct)).ShouldBe(JobOutcome.Succeeded, "kilit bırakıldıktan sonra yeniden çalışır");
    }

    private ServiceProvider Services(Gate? gate = null) => db.CreateServices(configure: services => services
        .AddSingleton(gate ?? new Gate())
        .AddPeriodicJob<SucceedingJob>()
        .AddPeriodicJob<FailingJob>()
        .AddPeriodicJob<BlockingJob>());

    private static JobRunner Runner(IServiceProvider services) => services.GetRequiredService<JobRunner>();

    public sealed class Gate
    {
        public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public sealed class SucceedingJob : IPeriodicJob
    {
        public string Key => "TestSucceedingJob";

        public Duration Interval => Duration.FromMinutes(1);

        public Task RunAsync(CancellationToken ct) => Task.CompletedTask;
    }

    public sealed class FailingJob : IPeriodicJob
    {
        public string Key => "TestFailingJob";

        public Duration Interval => Duration.FromMinutes(1);

        public Task RunAsync(CancellationToken ct) => throw new InvalidOperationException("iş hatası");
    }

    public sealed class BlockingJob(Gate gate) : IPeriodicJob
    {
        public string Key => "TestBlockingJob";

        public Duration Interval => Duration.FromMinutes(1);

        public async Task RunAsync(CancellationToken ct)
        {
            gate.Entered.TrySetResult();
            await gate.Release.Task.WaitAsync(ct);
        }
    }
}
