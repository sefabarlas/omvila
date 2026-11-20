namespace Omvila.Host;

/// <summary>
/// Aynı imajın çalışma rolü: <c>--role=api|worker|all</c> (docs/mimari/mimari.md § Genel resim).
/// Belirtilmezse <see cref="All"/> (geliştirme ve OpenAPI belge üretimi).
/// </summary>
[Flags]
internal enum HostRole
{
    Api = 1,
    Worker = 2,
    All = Api | Worker,
}
