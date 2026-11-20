using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Omvila.Infrastructure.Jobs;

/// <summary>Worker rolünde kayıtlı her işi kendi aralığıyla çalıştırır (BackgroundService + PeriodicTimer).</summary>
internal sealed partial class JobHostedService(
    IEnumerable<JobRegistration> registrations,
    IServiceScopeFactory scopes,
    JobRunner runner,
    ILogger<JobHostedService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.WhenAll(registrations.Select(registration => RunLoopAsync(registration.JobType, stoppingToken)));

    private async Task RunLoopAsync(Type jobType, CancellationToken ct)
    {
        TimeSpan interval;
        await using (var scope = scopes.CreateAsyncScope())
        {
            interval = ((IPeriodicJob)scope.ServiceProvider.GetRequiredService(jobType)).Interval.ToTimeSpan();
        }

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(ct))
        {
            // Geçici hata (veritabanına ulaşılamıyor) döngüyü ve uygulamayı durdurmaz; sonraki turda yeniden denenir.
            try
            {
                await runner.RunOnceAsync(jobType, ct);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogTickFailed(logger, exception, jobType.Name);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Job loop tick failed for {Job}")]
    private static partial void LogTickFailed(ILogger logger, Exception exception, string job);
}
