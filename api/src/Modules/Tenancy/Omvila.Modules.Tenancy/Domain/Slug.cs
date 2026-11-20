using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Rezervasyon bağlantı adı: <c>omvila.com/{slug}</c> (alan-modeli.md § Brand). Küçük harf, <c>[a-z0-9-]</c>,
/// 3–60 karakter (sözleşme BookingPageSettingsUpdate.slug); ayrılmış adlar kullanılamaz.
/// </summary>
internal sealed partial record Slug
{
    public const int MinLength = 3;
    public const int MaxLength = 60;

    // dilim-01 § Alan kuralları: r, b, d, api, giris, kayit, admin, panel, www… Kısa bağlantı ve site yolları.
    private static readonly HashSet<string> ReservedNames = new(StringComparer.Ordinal)
    {
        "r", "b", "d", "api", "giris", "kayit", "admin", "panel", "www",
        "app", "health", "openapi", "static", "assets", "destek", "yardim", "iletisim",
        "gizlilik", "kosullar", "kvkk", "fiyatlar", "blog", "omvila",
    };

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static bool IsReserved(string value) => ReservedNames.Contains(value);

    /// <summary>Biçim ve ayrılmış ad denetimi. Hata kodu: biçim için invalid_format, ayrılmış ad için slug_unavailable.</summary>
    public static Result<Slug> Create(string? value)
    {
        if (value is null || !Pattern().IsMatch(value))
        {
            return Error.Validation("slug", "invalid_format");
        }

        return IsReserved(value) ? Error.Conflict(ErrorCodes.SlugUnavailable) : new Slug(value);
    }

    public static bool TryCreate(string? value, [NotNullWhen(true)] out Slug? slug)
    {
        var result = Create(value);
        slug = result.IsSuccess ? result.Value : null;
        return slug is not null;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[a-z0-9-]{3,60}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex Pattern();
}
