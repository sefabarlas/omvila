namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Şifreli IBAN ve maskeli gösterim için açık son 4 hane.</summary>
internal sealed record EncryptedIban(byte[] Ciphertext, string Last4);
