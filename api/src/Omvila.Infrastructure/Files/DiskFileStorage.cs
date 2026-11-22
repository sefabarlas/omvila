using Microsoft.Extensions.Options;

namespace Omvila.Infrastructure.Files;

/// <summary>
/// Yerel disk deposu: <c>{RootPath}/{area}/{id}{ext}</c>. Anahtar yalnız sunucunun ürettiği parçalardan oluşur; okumada anahtar
/// kökün dışına çıkamaz (yol geçişi engeli).
/// </summary>
internal sealed class DiskFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    public async Task<string> SaveAsync(string area, Guid id, string extension, Stream content, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (!IsSafeSegment(area) || !extension.StartsWith('.') || !IsSafeSegment(extension[1..]))
        {
            throw new ArgumentException("Geçersiz dosya alanı veya uzantısı.", nameof(area));
        }

        var key = $"{area}/{id:N}{extension}";
        var path = Resolve(key) ?? throw new InvalidOperationException("Dosya yolu çözülemedi.");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await content.CopyToAsync(file, ct);
        return key;
    }

    public Task<Stream?> OpenReadAsync(string key, CancellationToken ct)
    {
        var path = Resolve(key);
        Stream? stream = path is not null && File.Exists(path)
            ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true)
            : null;
        return Task.FromResult(stream);
    }

    private static bool IsSafeSegment(string value) => value.Length > 0 && value.All(c => char.IsAsciiLetterOrDigit(c) || c == '-');

    private string? Resolve(string key)
    {
        var root = Path.GetFullPath(options.Value.RootPath);
        var full = Path.GetFullPath(Path.Combine(root, key));
        return full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal) ? full : null;
    }
}
