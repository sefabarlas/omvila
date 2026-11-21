using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Omvila.Infrastructure.Jobs;

/// <summary>
/// Worker rolünde kayıtlı her işi kendi aralığıyla çalıştırır (BackgroundService + PeriodicTimer; ADR-0005).
/// <list type="bullet">
/// <item>Aralıklar açılışta kayıttaki seçiciden (yapılandırma) okunur; iş oluşturulmaz, veritabanına bağlanılmaz.
/// Okunamayan veya geçersiz aralık (kurulum hatası) Critical günlüğe yazılır ve servis hata ile biter: host'un
/// BackgroundServiceExceptionBehavior'ı (varsayılan StopHost) uygulamayı durdurur.</item>
/// <item>Bir turdaki hata (veritabanına ulaşılamıyor, iş istisnası) geçicidir: Error günlüğe yazılır, sonraki turda
/// yeniden denenir; döngü ve uygulama sürer.</item>
/// <item>Bir döngü beklenmedik şekilde biterse Critical günlüğe yazılır, diğer döngüler durdurulur ve servis hata ile
/// biter (sessiz ölüm yok).</item>
/// </list>
/// </summary>
internal sealed partial class JobHostedService(
    IEnumerable<JobRegistration> registrations,
    IServiceProvider services,
    JobRunner runner,
    ILogger<JobHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var schedules = ResolveSchedules();
        if (schedules.Count == 0)
        {
            return;
        }

        using var loopsStopping = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var loops = schedules.Select(s => RunLoopAsync(s.JobType, s.Interval, loopsStopping.Token)).ToList();

        var first = await Task.WhenAny(loops);
        await loopsStopping.CancelAsync();
        await Task.WhenAll(loops).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        // Hata ile biten döngünün istisnası (RunLoopAsync Critical yazdı) servisi de hata ile bitirir.
        await first;
    }

    private List<(Type JobType, TimeSpan Interval)> ResolveSchedules()
    {
        var schedules = new List<(Type, TimeSpan)>();
        foreach (var registration in registrations)
        {
            Duration interval;
            try
            {
                interval = registration.ResolveInterval(services);
            }
            catch (Exception exception)
            {
                LogSetupFailed(logger, exception, registration.JobType.Name);
                throw;
            }

            if (interval <= Duration.Zero)
            {
                var exception = new InvalidOperationException(
                    $"Periyodik iş aralığı sıfırdan büyük olmalı: {registration.JobType.Name} ({interval}).");
                LogSetupFailed(logger, exception, registration.JobType.Name);
                throw exception;
            }

            schedules.Add((registration.JobType, interval.ToTimeSpan()));
        }

        return schedules;
    }

    private async Task RunLoopAsync(Type jobType, TimeSpan interval, CancellationToken ct)
    {
        try
        {
            using var timer = new PeriodicTimer(interval);
            while (await timer.WaitForNextTickAsync(ct))
            {
                // Geçici hata (veritabanına ulaşılamıyor, iş istisnası, dış servis zaman aşımı) döngüyü ve uygulamayı
                // durdurmaz; sonraki turda yeniden denenir. Yalnız durdurma isteği döngüyü bitirir.
                try
                {
                    await runner.RunOnceAsync(jobType, ct);
                }
                catch (Exception exception) when (!ct.IsCancellationRequested)
                {
                    LogTickFailed(logger, exception, jobType.Name);
                }
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return;
        }
        catch (Exception exception)
        {
            LogLoopFailed(logger, exception, jobType.Name);
            throw;
        }

        if (!ct.IsCancellationRequested)
        {
            var ended = new InvalidOperationException($"Periyodik iş döngüsü beklenmedik şekilde bitti: {jobType.Name}.");
            LogLoopFailed(logger, ended, jobType.Name);
            throw ended;
        }
    }

    [LoggerMessage(Level = LogLevel.Critical, Message = "Periodic job {Job} could not be scheduled (configuration); worker stops")]
    private static partial void LogSetupFailed(ILogger logger, Exception exception, string job);

    [LoggerMessage(Level = LogLevel.Error, Message = "Job loop tick failed for {Job}; retrying on next tick")]
    private static partial void LogTickFailed(ILogger logger, Exception exception, string job);

    [LoggerMessage(Level = LogLevel.Critical, Message = "Job loop for {Job} ended unexpectedly; worker stops")]
    private static partial void LogLoopFailed(ILogger logger, Exception exception, string job);
}
