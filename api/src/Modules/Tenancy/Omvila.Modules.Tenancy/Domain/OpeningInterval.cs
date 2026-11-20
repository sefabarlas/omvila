using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Yerel saat aralığı (şubenin saat diliminde). <c>Start &lt; End</c>; gece yarısını aşan aralık v1'de yoktur
/// (ADR-0003, alan-modeli.md § BranchWorkingHours).
/// </summary>
internal readonly record struct OpeningInterval
{
    private OpeningInterval(LocalTime start, LocalTime end)
    {
        Start = start;
        End = end;
    }

    public LocalTime Start { get; }

    public LocalTime End { get; }

    public static Result<OpeningInterval> Create(LocalTime start, LocalTime end, string field) =>
        start < end ? new OpeningInterval(start, end) : Error.Validation(field, "invalid_range");

    public bool Overlaps(OpeningInterval other) => Start < other.End && other.Start < End;
}
