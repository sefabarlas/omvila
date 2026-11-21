using System.Diagnostics.CodeAnalysis;

namespace Omvila.SharedKernel;

/// <summary>
/// Türkiye IBAN'ı (kapora hesabı, alan-modeli.md § BrandBankAccount). Boşluksuz ve büyük harfle saklanır
/// (<c>TR290006400000112345678901</c>): TR + 24 hane, toplam 26 karakter, mod-97 denetim hanesi geçerli.
/// Tam değer kişisel/finansal veridir: günlüğe yazılmaz; <see cref="ToString"/> maskeli biçimi döner.
/// </summary>
public sealed record Iban
{
    private const string CountryCode = "TR";
    private const int Length = 26;

    private Iban(string value) => Value = value;

    /// <summary>Boşluksuz, büyük harf: <c>TR290006400000112345678901</c>. Yalnız şifreleme ve müşteriye gösterim için.</summary>
    public string Value { get; }

    /// <summary>Maskeli gösterim (sözleşme BankAccount.ibanMasked): <c>TR29 •••• 6789 01</c>.</summary>
    public string Masked => $"{Value[..4]} •••• {Value[^6..^2]} {Value[^2..]}";

    /// <summary>Son 4 hane (sözleşme BankAccount.ibanLast4): <c>8901</c>.</summary>
    public string Last4 => Value[^4..];

    /// <summary>Dörtlü gruplu gösterim (sözleşme DepositInstructions.iban): <c>TR29 0006 4000 0011 2345 6789 01</c>.</summary>
    public string Grouped => string.Join(' ', Value.Chunk(4).Select(group => new string(group)));

    /// <summary>
    /// Serbest girişi çözer: boşluklar atılır, küçük harf büyütülür; TR biçimi ve mod-97 (ISO 13616) denetlenir.
    /// </summary>
    public static bool TryParse(string? input, [NotNullWhen(true)] out Iban? iban)
    {
        iban = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalized = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToUpperInvariant();
        if (normalized.Length != Length
            || !normalized.StartsWith(CountryCode, StringComparison.Ordinal)
            || !normalized.Skip(CountryCode.Length).All(char.IsAsciiDigit)
            || !HasValidCheckDigits(normalized))
        {
            return false;
        }

        iban = new Iban(normalized);
        return true;
    }

    /// <inheritdoc/>
    public override string ToString() => Masked;

    // ISO 13616: ilk dört karakter sona alınır, harfler 10–35 sayılarına çevrilir, kalan 97'ye bölümünden 1 olmalı.
    private static bool HasValidCheckDigits(string value)
    {
        var remainder = 0;
        foreach (var c in value[4..] + value[..4])
        {
            if (char.IsAsciiDigit(c))
            {
                remainder = ((remainder * 10) + (c - '0')) % 97;
            }
            else
            {
                remainder = ((remainder * 100) + (c - 'A' + 10)) % 97;
            }
        }

        return remainder == 1;
    }
}
