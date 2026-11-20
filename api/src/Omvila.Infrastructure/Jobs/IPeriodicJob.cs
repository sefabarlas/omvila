using NodaTime;

namespace Omvila.Infrastructure.Jobs;

/// <summary>
/// Periyodik iş (ADR-0005). İş mantığı modülün Jobs/ sınıfındadır ve IClock alır; testte doğrudan
/// JobRunner.RunOnceAsync ile çağrılır. Her çalışma kendi kapsamında (scope) çözülür.
/// </summary>
public interface IPeriodicJob
{
    /// <summary>Kararlı ad: advisory lock anahtarı ve job_runs kaydı (ör. "ExpireAppointmentHolds").</summary>
    string Key { get; }

    Duration Interval { get; }

    Task RunAsync(CancellationToken ct);
}
