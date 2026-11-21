using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Tests;

// Kural: ADR-0005 (zamanlayıcı): kurulum/yapılandırma hatası açılışta durdurur; tur hatası günlüğe yazılıp yeniden denenir.
public sealed class JobHostedServiceTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task MissingConfiguration_FaultsExecuteTask_AndLogsCritical()
    {
        var logs = new CapturingLoggerProvider();
        await using var services = Services(logs, s => s.AddOptions<ProbeOptions>()
            .Validate(o => o.IntervalMilliseconds > 0, "Probe ayarları eksik."));
        var worker = Worker(services);

        await worker.StartAsync(Ct);

        await Should.ThrowAsync<OptionsValidationException>(() => worker.ExecuteTask!.WaitAsync(Timeout, Ct));
        logs.Entries.ShouldContain(e => e.Level == LogLevel.Critical && e.Message.Contains(nameof(ProbeJob), StringComparison.Ordinal));
        await worker.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task NonPositiveInterval_FaultsExecuteTask()
    {
        var logs = new CapturingLoggerProvider();
        await using var services = Services(logs, s => s.Configure<ProbeOptions>(o => o.IntervalMilliseconds = 0));
        var worker = Worker(services);

        await worker.StartAsync(Ct);

        await Should.ThrowAsync<InvalidOperationException>(() => worker.ExecuteTask!.WaitAsync(Timeout, Ct));
        logs.Entries.ShouldContain(e => e.Level == LogLevel.Critical);
        await worker.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task TickFailure_IsLoggedAndRetried_WithoutStoppingTheWorker()
    {
        // Tur hatası: veritabanı oturumu çözülemiyor (veritabanına ulaşılamamasıyla aynı yol: JobRunner istisna atar).
        var logs = new CapturingLoggerProvider();
        await using var services = Services(logs, s => s.Configure<ProbeOptions>(o => o.IntervalMilliseconds = 50));
        var worker = Worker(services);

        await worker.StartAsync(Ct);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        deadline.CancelAfter(Timeout);
        while (logs.Entries.Count(e => e.Level == LogLevel.Error) < 2 && !deadline.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(20), Ct);
        }

        logs.Entries.Count(e => e.Level == LogLevel.Error).ShouldBeGreaterThanOrEqualTo(2, "hata her turda yazılır ve yeniden denenir");
        worker.ExecuteTask!.IsCompleted.ShouldBeFalse("tur hatası döngüyü durdurmamalı");
        logs.Entries.ShouldNotContain(e => e.Level == LogLevel.Critical);

        await worker.StopAsync(CancellationToken.None);
        await worker.ExecuteTask.WaitAsync(Timeout, Ct);
        worker.ExecuteTask.IsCompletedSuccessfully.ShouldBeTrue("durdurma isteği servisi hatasız bitirir");
    }

    private static ServiceProvider Services(CapturingLoggerProvider logs, Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection()
            .AddLogging(b => b.AddProvider(logs))
            .AddSingleton<IClock>(SystemClock.Instance)
            .AddSingleton<IIdGenerator, UuidV7IdGenerator>()
            .AddSingleton(NpgsqlDataSource.Create("Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=1"))
            .AddSingleton<JobRunner>()
            .AddPeriodicJob<ProbeJob, ProbeOptions>(o => Duration.FromMilliseconds(o.IntervalMilliseconds))
            .AddOmvilaWorker();
        configure(services);
        return services.BuildServiceProvider();
    }

    private static JobHostedService Worker(IServiceProvider services) =>
        services.GetServices<IHostedService>().OfType<JobHostedService>().Single();

    public sealed class ProbeOptions
    {
        public int IntervalMilliseconds { get; set; }
    }

    public sealed class ProbeJob : IPeriodicJob
    {
        public string Key => "TestProbeJob";

        public Task RunAsync(CancellationToken ct) => Task.CompletedTask;
    }

    private sealed record LogEntry(LogLevel Level, string Message);

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<LogEntry> _entries = new();

        public IReadOnlyCollection<LogEntry> Entries => _entries.ToArray();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(_entries);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(ConcurrentQueue<LogEntry> entries) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state)
                where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                entries.Enqueue(new LogEntry(logLevel, formatter(state, exception)));
        }
    }
}
