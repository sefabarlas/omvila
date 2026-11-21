namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Şubeler arası geçiş süresi (alan-modeli.md § BranchTransferTime): çifte özel değer yönlüdür (from → to); tanımlı
/// değilse marka varsayılanı (<c>DefaultBranchTransferMinutes</c>).
/// </summary>
internal sealed record TransferTimes(int DefaultMinutes, IReadOnlyDictionary<(Guid From, Guid To), int> Pairs)
{
    public static TransferTimes Uniform(int minutes) => new(minutes, new Dictionary<(Guid, Guid), int>());

    public NodaTime.Duration Between(Guid from, Guid to) =>
        from == to ? NodaTime.Duration.Zero
        : NodaTime.Duration.FromMinutes(Pairs.TryGetValue((from, to), out var minutes) ? minutes : DefaultMinutes);
}
