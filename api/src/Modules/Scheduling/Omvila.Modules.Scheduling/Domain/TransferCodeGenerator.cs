using System.Security.Cryptography;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kapora açıklama kodu (alan-modeli.md § Appointment.DepositTransferCode, string(12); dilim-04 "OMV-7K2X"): marka öneki
/// (2–5 büyük harf), tire ve 4 karakter. Alfabe karışan karakterleri içermez (0/O, 1/I). Kriptografik rastgele
/// (<see cref="RandomNumberGenerator"/>); 32^4 ≈ 1 milyon olasılık. Marka içinde benzersizlik veritabanı indeksiyle korunur,
/// çakışmada yazma yolu yeni kod üretir. Testte sıralı kod verebilmek için DI ile kaydedilir.
/// </summary>
internal class TransferCodeGenerator
{
    /// <summary>alan-modeli.md: DepositTransferCode string(12).</summary>
    public const int MaxLength = 12;

    public const int RandomLength = 4;

    public const string Alphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

    public virtual string New(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        return $"{prefix}-{RandomNumberGenerator.GetString(Alphabet, RandomLength)}";
    }
}
