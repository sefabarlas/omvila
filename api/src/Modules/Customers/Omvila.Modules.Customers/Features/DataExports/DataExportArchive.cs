using System.IO.Compression;
using System.Text;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain.Import;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>
/// Dışa aktarma arşivi: her katılımcı tablosu bir CSV (müşteri dışa aktarmasıyla aynı biçim: UTF-8 BOM, noktalı virgül,
/// CSV enjeksiyonuna karşı kaçış), hepsi tek ZIP'te. Dosya adları sabittir; aynı ad iki kez gelirse programcı hatasıdır.
/// </summary>
internal static class DataExportArchive
{
    public const char Delimiter = ';';

    public static byte[] Build(IEnumerable<BrandDataExportFile> files)
    {
        using var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in files.OrderBy(f => f.FileName, StringComparer.Ordinal))
            {
                var entry = zip.CreateEntry(file.FileName, CompressionLevel.Optimal);
                using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                writer.Write(Csv(file));
            }
        }

        return stream.ToArray();
    }

    public static string Csv(BrandDataExportFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        var csv = new StringBuilder();
        csv.AppendJoin(Delimiter, file.Headers.Select(h => CsvText.Field(h, Delimiter))).Append("\r\n");
        foreach (var row in file.Rows)
        {
            csv.AppendJoin(Delimiter, row.Select(v => CsvText.Field(v, Delimiter))).Append("\r\n");
        }

        return csv.ToString();
    }
}
