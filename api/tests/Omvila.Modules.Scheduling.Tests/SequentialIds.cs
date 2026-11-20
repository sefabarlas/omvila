using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

/// <summary>Testin sıralı kimlik üreticisi (Guid üretimi IIdGenerator'a aittir).</summary>
internal sealed class SequentialIds : IIdGenerator
{
    private int _next;

    public Guid New() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
}
