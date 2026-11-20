using System.Security.Cryptography;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// SMS bağlantısı kodu (alan-modeli.md § AppointmentAccessCode): 10 karakter Base62, kriptografik rastgele
/// (<see cref="RandomNumberGenerator"/>; System.Random yasak). 62^10 ≈ 8,4 × 10^17 olasılık; benzersizlik ayrıca
/// veritabanı indeksiyle korunur.
/// </summary>
internal static class AccessCodeGenerator
{
    /// <summary>alan-modeli.md § AppointmentAccessCode.Code: string(10).</summary>
    public const int Length = 10;

    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string New() => RandomNumberGenerator.GetString(Alphabet, Length);

    public static bool IsWellFormed(string? code) =>
        code is { Length: Length } && code.All(c => char.IsAsciiLetterOrDigit(c));
}
