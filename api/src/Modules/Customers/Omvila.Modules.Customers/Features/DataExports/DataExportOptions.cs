namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>"Customers:DataExport" (T-121): dışa aktarma işinin aralığı ve indirme bağlantısının imza anahtarı; saklama işinin aralığı.</summary>
internal sealed class DataExportOptions
{
    public const string Section = "Customers:DataExport";

    public const string LinkSigningKeySetting = Section + ":LinkSigningKey";

    /// <summary>ProcessDataExports işinin aralığı (saniye).</summary>
    public int ProcessIntervalSeconds { get; set; }

    /// <summary>ApplyRetention işinin aralığı (dakika; mimari.md: günlük).</summary>
    public int ApplyRetentionIntervalMinutes { get; set; }

    /// <summary>
    /// İndirme bağlantısı imza anahtarı (HMAC-SHA256; base64, 32 bayt). Gizli değer: ortam değişkeni
    /// <c>Customers__DataExport__LinkSigningKey</c>. Development ve Testing'de boşsa geçici anahtar; Staging/Production'da zorunlu.
    /// </summary>
    public string? LinkSigningKey { get; set; }

    internal bool IsValid() => ProcessIntervalSeconds > 0 && ApplyRetentionIntervalMinutes > 0;
}
