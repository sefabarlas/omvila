using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Kaporanın gönderileceği salon hesabı (alan-modeli.md § BrandBankAccount; markada tek satır, BrandId PK).
/// OMVILA bu hesaba para göndermez veya hesaptan çekmez; yalnız müşteriye gösterir. IBAN uygulama düzeyinde şifreli
/// saklanır (guvenlik-kvkk.md); maske ve son 4 hane için yalnız <see cref="IbanLast4"/> açıktır.
/// </summary>
internal sealed class BrandBankAccount
{
    private BrandBankAccount()
    {
    }

    public Guid BrandId { get; private set; }

    public string AccountHolderName { get; private set; } = string.Empty;

    public string BankName { get; private set; } = string.Empty;

    /// <summary>AES-256-GCM şifreli IBAN (IbanProtector biçimi; marka kimliğine bağlı).</summary>
    public byte[] IbanEncrypted { get; private set; } = [];

    public string IbanLast4 { get; private set; } = string.Empty;

    public string TransferCodePrefix { get; private set; } = string.Empty;

    public int TransferWindowMinutes { get; private set; }

    public bool IsActive { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant UpdatedUtc { get; private set; }

    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    /// <summary>İlk kayıt ("Kaydet", If-Match W/"0"). Kayıttan sonra hesap aktiftir (sözleşme manageUpdateBankAccount).</summary>
    public static BrandBankAccount Create(Guid brandId, BankAccountValues values, EncryptedIban iban, Instant now)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(iban);
        var account = new BrandBankAccount { BrandId = brandId, CreatedUtc = now };
        account.Update(values, iban, now);
        return account;
    }

    /// <summary>Güncelleme; <paramref name="iban"/> null ise kayıtlı IBAN korunur. Havale süresi yalnız yeni randevulara uygulanır.</summary>
    public void Update(BankAccountValues values, EncryptedIban? iban, Instant now)
    {
        ArgumentNullException.ThrowIfNull(values);
        AccountHolderName = values.AccountHolderName.Trim();
        BankName = values.BankName.Trim();
        TransferCodePrefix = values.TransferCodePrefix;
        TransferWindowMinutes = values.TransferWindowMinutes;
        if (iban is not null)
        {
            IbanEncrypted = iban.Ciphertext;
            IbanLast4 = iban.Last4;
        }

        IsActive = true;
        UpdatedUtc = now;
    }
}
