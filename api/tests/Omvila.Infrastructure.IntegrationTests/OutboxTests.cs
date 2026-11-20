using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: ADR-0005 (outbox aynı transaction'da; en az bir kez teslim; işleyici idempotent: processed_events)
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class OutboxTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync() => await InfraTables.ResetAsync(db, Ct);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Enqueue_InCommittedTransaction_PersistsEvent()
    {
        await using var services = Services(new Recorder());

        await EnqueueAsync(services, new SomethingHappened(BrandA, "ilk"), commit: true);

        (await CountAsync("SELECT count(*) FROM infra.outbox_events")).ShouldBe(1);
    }

    [Fact]
    public async Task Enqueue_InRolledBackTransaction_LeavesNoEvent()
    {
        await using var services = Services(new Recorder());

        await EnqueueAsync(services, new SomethingHappened(BrandA, "geri alınan"), commit: false);

        (await CountAsync("SELECT count(*) FROM infra.outbox_events")).ShouldBe(0);
    }

    [Fact]
    public async Task Dispatch_DeliversToEveryHandlerOnce_AndMarksProcessed()
    {
        var recorder = new Recorder();
        await using var services = Services(recorder);
        await EnqueueAsync(services, new SomethingHappened(BrandA, "randevu"), commit: true);

        await DispatchAsync(services);
        await DispatchAsync(services);

        recorder.Calls.OrderBy(c => c, StringComparer.Ordinal).ShouldBe(["A:randevu", "B:randevu"]);
        (await CountAsync("SELECT count(*) FROM infra.outbox_events WHERE processed_utc IS NOT NULL")).ShouldBe(1);
        (await CountAsync("SELECT count(*) FROM infra.processed_events")).ShouldBe(2);
    }

    [Fact]
    public async Task HandlerFailure_RollsBackItsWrites_AndRetriesOnlyThatHandlerAfterBackoff()
    {
        var recorder = new Recorder { FailuresLeftForB = 1 };
        await using var services = Services(recorder);
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        await EnqueueAsync(services, new SomethingHappened(BrandA, "kapora"), commit: true);

        await DispatchAsync(services);

        recorder.Calls.ShouldBe(["A:kapora"]);
        (await CountAsync("SELECT attempts::bigint FROM infra.outbox_events")).ShouldBe(1);
        (await CountAsync("SELECT count(*) FROM infra.outbox_events WHERE processed_utc IS NULL")).ShouldBe(1);
        // B hata vermeden önce outbox'a yazdığı olay, B'nin transaction'ıyla birlikte geri alındı.
        (await CountAsync("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%FollowUpRequested'")).ShouldBe(0);

        await DispatchAsync(services);
        recorder.Calls.Count.ShouldBe(1, "geri çekilme süresi dolmadan yeniden denenmez");

        clock.Advance(Duration.FromSeconds(30));
        await DispatchAsync(services);

        recorder.Calls.OrderBy(c => c, StringComparer.Ordinal).ShouldBe(["A:kapora", "B:kapora"]);
        (await CountAsync("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%SomethingHappened' AND processed_utc IS NOT NULL")).ShouldBe(1);
    }

    [Fact]
    public async Task EventWithoutHandlers_IsMarkedProcessed()
    {
        await using var services = Services(new Recorder());
        await EnqueueAsync(services, new NobodyListens(BrandA), commit: true);

        await DispatchAsync(services);

        (await CountAsync("SELECT count(*) FROM infra.outbox_events WHERE processed_utc IS NULL")).ShouldBe(0);
    }

    private ServiceProvider Services(Recorder recorder) => db.CreateServices(configure: services => services
        .AddSingleton(recorder)
        .AddIntegrationEventHandler<SomethingHappened, HandlerA>()
        .AddIntegrationEventHandler<SomethingHappened, HandlerB>());

    private static async Task EnqueueAsync<TEvent>(IServiceProvider services, TEvent integrationEvent, bool commit)
        where TEvent : IIntegrationEvent
    {
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
        await session.BeginAsync(Ct);
        await scope.ServiceProvider.GetRequiredService<IOutbox>().EnqueueAsync(integrationEvent, Ct);
        if (commit)
        {
            await session.CommitAsync(Ct);
        }
        else
        {
            await session.RollbackAsync(Ct);
        }
    }

    private static async Task DispatchAsync(IServiceProvider services) =>
        (await services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(OutboxDispatcher), Ct)).ShouldBe(JobOutcome.Succeeded);

    private Task<long> CountAsync(string sql) => InfraTables.ScalarAsync<long>(db, sql, Ct);

    public sealed record SomethingHappened(Guid BrandId, string Note) : IIntegrationEvent;

    public sealed record FollowUpRequested(Guid BrandId) : IIntegrationEvent;

    public sealed record NobodyListens(Guid BrandId) : IIntegrationEvent;

    public sealed class Recorder
    {
        public ConcurrentQueue<string> Calls { get; } = new();

        public int FailuresLeftForB { get; set; }
    }

    public sealed class HandlerA(Recorder recorder) : IIntegrationEventHandler<SomethingHappened>
    {
        public Task HandleAsync(SomethingHappened integrationEvent, CancellationToken ct)
        {
            recorder.Calls.Enqueue($"A:{integrationEvent.Note}");
            return Task.CompletedTask;
        }
    }

    public sealed class HandlerB(Recorder recorder, IOutbox outbox) : IIntegrationEventHandler<SomethingHappened>
    {
        public async Task HandleAsync(SomethingHappened integrationEvent, CancellationToken ct)
        {
            if (recorder.FailuresLeftForB > 0)
            {
                recorder.FailuresLeftForB--;
                await outbox.EnqueueAsync(new FollowUpRequested(integrationEvent.BrandId), ct);
                throw new InvalidOperationException("geçici hata");
            }

            recorder.Calls.Enqueue($"B:{integrationEvent.Note}");
        }
    }
}
