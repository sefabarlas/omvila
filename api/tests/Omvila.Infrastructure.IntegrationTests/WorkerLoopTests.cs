using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Omvila.Infrastructure.Jobs;
using Omvila.Modules.Notifications;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: ADR-0005 (worker rolünde kayıtlı her iş kendi aralığıyla döngüde çalışır; çalışma kaydı infra.job_runs).
// JobRunner testleri işi doğrudan çağırır; bu test zamanlayıcının (JobHostedService) kurucusunda modül DbContext'i
// alan gerçek bir işi (DispatchNotifications) gerçekten çalıştırdığını doğrular.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class WorkerLoopTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly TimeSpan Deadline = TimeSpan.FromSeconds(20);

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync() => await InfraTables.ResetAsync(db, Ct);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task WorkerLoop_RunsJobThatTakesModuleDbContext()
    {
        await using var services = db.CreateServices(configure: s => s
            .AddNotificationsModule(db.Configuration())
            .AddOmvilaWorker());
        var worker = services.GetServices<IHostedService>().OfType<JobHostedService>().Single();

        await worker.StartAsync(Ct);
        long runs;
        try
        {
            runs = await WaitForSucceededRunAsync("DispatchNotifications");
            worker.ExecuteTask!.IsFaulted.ShouldBeFalse("döngü görevi hata ile bitmemeli");
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }

        runs.ShouldBeGreaterThan(0, "DispatchNotifications döngüde en az bir kez başarıyla çalışmalı (infra.job_runs)");
    }

    private async Task<long> WaitForSucceededRunAsync(string jobKey)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        deadline.CancelAfter(Deadline);
        while (!deadline.IsCancellationRequested)
        {
            var count = await InfraTables.ScalarAsync<long>(
                db, $"SELECT count(*) FROM infra.job_runs WHERE job_key = '{jobKey}' AND status = 'Succeeded'", Ct);
            if (count > 0)
            {
                return count;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250), Ct);
        }

        return 0;
    }
}
