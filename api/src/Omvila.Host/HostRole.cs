namespace Omvila.Host;

/// <summary>
/// Aynı imajın çalışma rolü: <c>--role=api|worker|all|seed</c> (docs/mimari/mimari.md § Genel resim).
/// Belirtilmezse <see cref="All"/> (geliştirme ve OpenAPI belge üretimi). <see cref="Seed"/> demo verisini yazıp çıkar
/// (Seeding/DemoSeedCommand; acik-sorular.md T-74); uç eşlemez, iş zamanlamaz.
/// </summary>
[Flags]
internal enum HostRole
{
    Api = 1,
    Worker = 2,
    All = Api | Worker,
    Seed = 4,
}
