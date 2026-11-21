namespace Omvila.Host;

/// <summary>
/// Aynı imajın çalışma rolü: <c>--role=api|worker|all|migrate</c> (docs/mimari/mimari.md § Genel resim).
/// Belirtilmezse <see cref="All"/> (geliştirme ve OpenAPI belge üretimi). <see cref="Migrate"/> migration'ları
/// uygulayıp çıkar; <see cref="All"/>'a dahil değildir.
/// </summary>
[Flags]
internal enum HostRole
{
    Api = 1,
    Worker = 2,
    All = Api | Worker,
    Migrate = 4,
}
