namespace Omvila.Modules.Tenancy.DemoData;

/// <summary>Demo kapora hesabı (AyarlarOdeme alanları). <paramref name="Iban"/> mod-97 geçerli uydurma IBAN olmalı.</summary>
public sealed record DemoBankAccount(string AccountHolderName, string BankName, string Iban, string TransferCodePrefix, int TransferWindowMinutes);
