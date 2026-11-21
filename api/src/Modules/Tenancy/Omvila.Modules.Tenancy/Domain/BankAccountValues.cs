namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Formdaki (sözleşme BankAccountWrite) IBAN dışı alanlar.</summary>
internal sealed record BankAccountValues(string AccountHolderName, string BankName, string TransferCodePrefix, int TransferWindowMinutes);
