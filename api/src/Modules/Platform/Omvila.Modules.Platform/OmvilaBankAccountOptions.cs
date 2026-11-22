namespace Omvila.Modules.Platform;

/// <summary>OMVILA'nın havale alacağı hesap (sözleşme OmvilaBankAccount). Unvan ve IBAN birlikte verilir.</summary>
public sealed class OmvilaBankAccountOptions
{
    public string? AccountHolderName { get; set; }

    /// <summary>Boşluksuz TR IBAN (TR + 24 rakam).</summary>
    public string? Iban { get; set; }

    public string? BankName { get; set; }

    internal bool IsConfigured => !string.IsNullOrWhiteSpace(AccountHolderName) && !string.IsNullOrWhiteSpace(Iban);

    internal bool IsEmptyOrValid()
    {
        if (string.IsNullOrWhiteSpace(AccountHolderName) && string.IsNullOrWhiteSpace(Iban))
        {
            return true;
        }

        return IsConfigured && Iban!.Length == 26 && Iban.StartsWith("TR", StringComparison.Ordinal) && Iban[2..].All(char.IsAsciiDigit);
    }
}
