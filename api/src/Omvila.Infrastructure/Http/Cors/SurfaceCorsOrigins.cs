namespace Omvila.Infrastructure.Http.Cors;

/// <summary>Bir yüzeyin izinli kökenleri ("Cors:Surfaces:{Yüzey}:Origins"); her biri şema://alan[:port].</summary>
public sealed class SurfaceCorsOrigins
{
    public IList<string> Origins { get; init; } = [];
}
