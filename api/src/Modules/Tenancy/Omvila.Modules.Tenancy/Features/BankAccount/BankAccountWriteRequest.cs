namespace Omvila.Modules.Tenancy.Features.BankAccount;

/// <summary>Sözleşme BankAccountWrite ("Kaydet"). iban ilk kayıtta zorunlu, sonra isteğe bağlı.</summary>
internal sealed record BankAccountWriteRequest(
    string? AccountHolderName, string? BankName, string? Iban, string? TransferCodePrefix, int? TransferWindowMinutes);
