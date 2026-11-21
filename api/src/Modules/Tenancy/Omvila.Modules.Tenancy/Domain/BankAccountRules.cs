using System.Text.RegularExpressions;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Kapora hesabı biçim kuralları (sözleşme BankAccountWrite; alan-modeli.md § BrandBankAccount; sureler-ve-ayarlar.md § Kapora):
/// alıcı adı 2–120, banka 2–80, önek 2–5 büyük Latin harfi, havale süresi 60/120/240; IBAN ilk kayıtta zorunlu, gelirse
/// TR biçimi ve mod-97. Alan yolu → hata kodu döner; boşsa geçerlidir.
/// </summary>
internal static partial class BankAccountRules
{
    public const int AccountHolderNameMaxLength = 120;
    public const int BankNameMaxLength = 80;
    private const int MinNameLength = 2;

    private static readonly int[] TransferWindows = [60, 120, 240];

    public static Dictionary<string, string[]> Validate(
        string? accountHolderName, string? bankName, string? transferCodePrefix, int? transferWindowMinutes,
        string? iban, bool isFirstSave, out Iban? parsedIban)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        Name("accountHolderName", accountHolderName, AccountHolderNameMaxLength, errors);
        Name("bankName", bankName, BankNameMaxLength, errors);

        if (transferCodePrefix is null)
        {
            errors["transferCodePrefix"] = ["required"];
        }
        else if (!PrefixPattern().IsMatch(transferCodePrefix))
        {
            errors["transferCodePrefix"] = ["invalid_format"];
        }

        if (transferWindowMinutes is null)
        {
            errors["transferWindowMinutes"] = ["required"];
        }
        else if (!TransferWindows.Contains(transferWindowMinutes.Value))
        {
            errors["transferWindowMinutes"] = ["out_of_range"];
        }

        parsedIban = null;
        if (iban is null)
        {
            if (isFirstSave)
            {
                errors["iban"] = ["required"];
            }
        }
        else if (!Iban.TryParse(iban, out parsedIban))
        {
            errors["iban"] = ["invalid_format"];
        }

        return errors;
    }

    private static void Name(string field, string? value, int max, Dictionary<string, string[]> errors)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            errors[field] = ["required"];
        }
        else if (trimmed.Length < MinNameLength)
        {
            errors[field] = ["too_short"];
        }
        else if (trimmed.Length > max)
        {
            errors[field] = ["too_long"];
        }
    }

    [GeneratedRegex("^[A-Z]{2,5}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex PrefixPattern();
}
