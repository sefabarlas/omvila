using System.Diagnostics.CodeAnalysis;

namespace Omvila.SharedKernel;

/// <summary>
/// Türkiye telefon numarası. E.164 saklanır (<c>+905324187705</c>), <c>0532 418 77 05</c> gösterilir.
/// v1'de yalnız +90 numaraları kabul edilir (docs/acik-sorular.md T-17).
/// </summary>
public sealed record PhoneNumber
{
    private const string CountryCode = "90";
    private const int NationalLength = 10;

    private PhoneNumber(string national) => National = national;

    /// <summary>Başında 0 olmadan 10 haneli ulusal numara: <c>5324187705</c>.</summary>
    public string National { get; }

    /// <summary>Saklanan biçim: <c>+905324187705</c>.</summary>
    public string E164 => $"+{CountryCode}{National}";

    /// <summary>Gösterim: <c>0532 418 77 05</c>.</summary>
    public string Display => $"0{National[..3]} {National[3..6]} {National[6..8]} {National[8..]}";

    /// <summary>Maskeli gösterim (Personel rolü, günlük): <c>0532 *** ** 05</c>.</summary>
    public string Masked => $"0{National[..3]} *** ** {National[8..]}";

    /// <summary>Cep telefonu mu (5xx). SMS ile giriş yalnız cep telefonuyla yapılır.</summary>
    public bool IsMobile => National[0] == '5';

    /// <summary>
    /// Serbest girişi çözer: "0532 418 77 05", "5324187705", "+90 532 418 7705", "0090 532…", "(0532) 418-77-05".
    /// </summary>
    public static bool TryParse(string? input, [NotNullWhen(true)] out PhoneNumber? phone)
    {
        phone = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var trimmed = input.Trim();
        var hasPlus = trimmed.StartsWith('+');
        var digits = new string(trimmed.Where(char.IsAsciiDigit).ToArray());
        if (digits.Length == 0 || trimmed.Any(c => !IsAllowedSeparator(c) && !char.IsAsciiDigit(c)))
        {
            return false;
        }

        string national;
        if (hasPlus || digits.StartsWith("00", StringComparison.Ordinal))
        {
            var international = hasPlus ? digits : digits[2..];
            if (!international.StartsWith(CountryCode, StringComparison.Ordinal))
            {
                return false;
            }

            national = international[CountryCode.Length..];
        }
        else if (digits.Length == NationalLength + 2 && digits.StartsWith(CountryCode, StringComparison.Ordinal))
        {
            national = digits[CountryCode.Length..];
        }
        else if (digits.Length == NationalLength + 1 && digits[0] == '0')
        {
            national = digits[1..];
        }
        else
        {
            national = digits;
        }

        // Türkiye'de ulusal numara 2, 3, 4 (sabit hat) veya 5 (cep) ile başlar.
        if (national.Length != NationalLength || national[0] is < '2' or > '5')
        {
            return false;
        }

        phone = new PhoneNumber(national);
        return true;
    }

    public static PhoneNumber Parse(string input) =>
        TryParse(input, out var phone) ? phone : throw new DomainException("Geçersiz telefon numarası.");

    /// <summary>Günlüğe ve hata mesajına tam numara yazılmasın diye maskeli döner.</summary>
    public override string ToString() => Masked;

    private static bool IsAllowedSeparator(char c) => c is ' ' or '-' or '(' or ')' or '.' or '+';
}
