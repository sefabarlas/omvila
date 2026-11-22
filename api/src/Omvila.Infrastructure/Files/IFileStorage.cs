namespace Omvila.Infrastructure.Files;

/// <summary>
/// Yüklenen dosyalar (dekont; ileride logo). Dosya anahtarla saklanır, anahtar veritabanında tutulur. Şimdilik yerel disk
/// (<see cref="DiskFileStorage"/>); barındırma kararıyla (T-07) nesne deposu gelir. Dosyalar API'den değil, yetkili uçtan sunulur
/// (guvenlik-kvkk.md § Dosya yükleme: ayrı alan adı canlıya çıkışta; T-118).
/// </summary>
public interface IFileStorage
{
    /// <summary>Dosyayı kaydeder ve anahtarını döner.</summary>
    /// <param name="area">Alan (ör. "payment-notices"); anahtarın öneki.</param>
    /// <param name="id">Dosya kimliği (anahtarın son parçası).</param>
    /// <param name="extension">Uzantı (".pdf").</param>
    /// <param name="content">İçerik.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<string> SaveAsync(string area, Guid id, string extension, Stream content, CancellationToken ct);

    /// <summary>Dosyayı okumak için açar; yoksa null.</summary>
    /// <param name="key">Kaydederken dönen anahtar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Stream?> OpenReadAsync(string key, CancellationToken ct);
}
