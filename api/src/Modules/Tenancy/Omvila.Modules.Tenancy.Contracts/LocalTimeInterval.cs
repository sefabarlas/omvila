using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Bir gün içindeki yerel saat aralığı [Start, End).</summary>
/// <param name="Start">Başlangıç.</param>
/// <param name="End">Bitiş.</param>
public sealed record LocalTimeInterval(LocalTime Start, LocalTime End);
