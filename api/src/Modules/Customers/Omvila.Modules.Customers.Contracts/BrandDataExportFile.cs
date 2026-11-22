using System.Globalization;
using NodaTime;
using NodaTime.Text;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Dışa aktarma arşivindeki bir tablo (CSV'ye Customers modülü çevirir).</summary>
/// <param name="FileName">Arşivdeki dosya adı (ör. <c>randevular.csv</c>).</param>
/// <param name="Headers">Türkçe sütun başlıkları.</param>
/// <param name="Rows">Satırlar; her satır başlık sayısı kadar değer.</param>
public sealed record BrandDataExportFile(string FileName, IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<string?>> Rows)
{
    /// <summary>Kuruştan "1234,50" (ondalık sayı hesabı yok; Türkçe elektronik tablo biçimi).</summary>
    /// <param name="amountMinor">Kuruş.</param>
    public static string Lira(long amountMinor) =>
        string.Create(CultureInfo.InvariantCulture, $"{(amountMinor < 0 ? "-" : string.Empty)}{Math.Abs(amountMinor) / 100},{Math.Abs(amountMinor) % 100:D2}");

    /// <summary>An, UTC ISO 8601 ("2026-11-22T10:00:00Z"); dosyalardaki bütün anlar UTC'dir.</summary>
    /// <param name="instant">An.</param>
    public static string? Utc(Instant? instant) => instant is { } value ? InstantPattern.General.Format(value) : null;
}
