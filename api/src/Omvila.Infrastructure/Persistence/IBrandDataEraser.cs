namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// Bir markanın bir modüldeki bütün satırlarını siler (yalnız demo verisi <c>--role=seed --reset</c>; acik-sorular.md T-74).
/// Her modül yalnız kendi DbContext'i için kaydeder (<see cref="BrandDataEraserExtensions.AddBrandDataEraser{TContext}"/>):
/// modül sınırı delinmez, silme isteğin kiracı bağlamında (RLS) çalışır.
/// </summary>
public interface IBrandDataEraser
{
    /// <summary>Markanın satırlarını siler; silinen satır sayısını döner.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> EraseAsync(Guid brandId, CancellationToken ct);
}
