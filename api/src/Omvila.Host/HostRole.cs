namespace Omvila.Host;

/// <summary>
/// Aynı imajın çalışma rolü: <c>--role=api|worker|all|migrate|seed|platform-user</c> (docs/mimari/mimari.md § Genel resim).
/// Belirtilmezse <see cref="All"/> (geliştirme ve OpenAPI belge üretimi). <see cref="Migrate"/> migration'ları
/// uygulayıp çıkar; <see cref="Seed"/> demo verisini yazıp çıkar (Seeding/DemoSeedCommand; acik-sorular.md T-74).
/// <see cref="PlatformUser"/> platform kullanıcısını oluşturup çıkar (PlatformUsers/PlatformUserCommand; K2, T-119).
/// Üçü de <see cref="All"/>'a dahil değildir; uç eşlemez, iş zamanlamaz.
/// </summary>
[Flags]
internal enum HostRole
{
    Api = 1,
    Worker = 2,
    All = Api | Worker,
    Migrate = 4,
    Seed = 8,
    PlatformUser = 16,
}
