namespace Omvila.Modules.Tenancy.Domain;

/// <summary>KVKK ve veri ayarlarının kuralları (sureler-ve-ayarlar.md § KVKK ve saklama; sözleşme PrivacySettingsUpdate).</summary>
internal static class PrivacySettingsRules
{
    public const string Standard = "Standard";
    public const string Custom = "Custom";
    public const int CustomTextMaxLength = 20000;

    /// <summary>TransactionRetentionYears izinli değerleri.</summary>
    public static IReadOnlyList<int> AllowedRetentionYears => Omvila.SharedKernel.PlatformDefaults.TransactionRetentionYearsAllowed;

    /// <summary>Biçim ve kural hataları (alan → kod).</summary>
    public static Dictionary<string, string[]> Validate(string? mode, bool textProvided, string? customText, string? currentText, int? retentionYears)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (mode is null)
        {
            errors["noticeMode"] = ["required"];
        }
        else if (mode is not (Standard or Custom))
        {
            errors["noticeMode"] = ["invalid_format"];
        }

        if (customText is { Length: > CustomTextMaxLength })
        {
            errors["customNoticeText"] = ["too_long"];
        }
        else if (string.Equals(mode, Custom, StringComparison.Ordinal) && string.IsNullOrWhiteSpace(textProvided ? customText : currentText))
        {
            errors["customNoticeText"] = ["required"];
        }

        if (retentionYears is null)
        {
            errors["transactionRetentionYears"] = ["required"];
        }
        else if (!AllowedRetentionYears.Contains(retentionYears.Value))
        {
            errors["transactionRetentionYears"] = ["out_of_range"];
        }

        return errors;
    }
}
