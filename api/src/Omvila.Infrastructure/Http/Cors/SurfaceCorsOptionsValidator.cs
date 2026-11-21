using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Omvila.Infrastructure.Http.Cors;

/// <summary>
/// Başlangıçta denetlenir: bilinmeyen yüzey adı, joker ("*"), yol/sorgu taşıyan veya mutlak olmayan köken reddedilir.
/// Development ve Testing dışında kökenler https olmalıdır.
/// </summary>
internal sealed class SurfaceCorsOptionsValidator(IHostEnvironment environment) : IValidateOptions<SurfaceCorsOptions>
{
    public ValidateOptionsResult Validate(string? name, SurfaceCorsOptions options)
    {
        var requireHttps = !environment.IsDevelopment() && !environment.IsEnvironment("Testing");
        var errors = Collect(options, requireHttps).ToList();
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }

    internal static IEnumerable<string> Collect(SurfaceCorsOptions options, bool requireHttps)
    {
        if (options.PreflightMaxAgeSeconds <= 0)
        {
            yield return "Cors:PreflightMaxAgeSeconds pozitif olmalı.";
        }

        foreach (var (surfaceName, entry) in options.Surfaces)
        {
            if (!ApiSurface.All.Any(s => s.Name.Equals(surfaceName, StringComparison.OrdinalIgnoreCase)))
            {
                yield return $"Cors:Surfaces:{surfaceName} bilinmeyen yüzey (Manage, Admin, Me, Public).";
                continue;
            }

            foreach (var origin in entry.Origins)
            {
                if (OriginError(origin, requireHttps) is { } error)
                {
                    yield return $"Cors:Surfaces:{surfaceName}:Origins '{origin}': {error}";
                }
            }
        }
    }

    private static string? OriginError(string origin, bool requireHttps)
    {
        if (string.IsNullOrWhiteSpace(origin) || origin.Contains('*', StringComparison.Ordinal))
        {
            return "joker veya boş köken kullanılamaz.";
        }

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
        {
            return "mutlak http(s) adresi olmalı.";
        }

        var isHttps = string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal);
        if (!isHttps && !string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal))
        {
            return "mutlak http(s) adresi olmalı.";
        }

        if (!string.Equals(uri.PathAndQuery, "/", StringComparison.Ordinal) || origin.EndsWith('/')
            || !string.IsNullOrEmpty(uri.Fragment) || !string.IsNullOrEmpty(uri.UserInfo))
        {
            return "yalnız köken yazılır (şema://alan[:port]), yol veya sonda '/' olmaz.";
        }

        return requireHttps && !isHttps ? "bu ortamda https olmalı." : null;
    }
}
