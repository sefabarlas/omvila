namespace Omvila.Infrastructure.Sms;

/// <summary>appsettings "Sms:DevLog": sahte SMS günlüğü sayfası (/dev/sms, T-76). Teknik ayar; ürün süresi değildir.</summary>
public sealed class DevSmsLogOptions
{
    public const string Section = "Sms:DevLog";

    /// <summary>Sayfada gösterilen en fazla kayıt (en yeni üstte).</summary>
    public int Limit { get; set; }

    /// <summary>Sayfanın kendini yenileme aralığı, saniye; 0 → yenilemez.</summary>
    public int RefreshSeconds { get; set; }

    public bool IsValid() => Limit is >= 1 and <= 1000 && RefreshSeconds is >= 0 and <= 3600;
}
