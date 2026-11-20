using NodaTime;

namespace Omvila.Infrastructure.Jobs;

/// <summary>infra.job_runs: periyodik işin her çalışması (ADR-0005).</summary>
internal sealed class JobRun
{
    public Guid Id { get; init; }

    public required string JobKey { get; init; }

    public Instant StartedUtc { get; init; }

    public Instant FinishedUtc { get; set; }

    public required string Status { get; set; }

    public string? Error { get; set; }
}
