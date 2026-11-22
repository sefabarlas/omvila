using System.Globalization;
using System.Text;
using NodaTime;
using NodaTime.Text;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Banka dökümü CSV'si (sözleşme adminImportBankTransactions; T-124). Genel biçim: UTF-8 (BOM'lu veya BOM'suz), ilk satır başlık,
/// ayraç noktalı virgül veya virgül (başlıktan anlaşılır), alanlar çift tırnakla sarılabilir. Sütunlar başlık adından eşlenir
/// (büyük/küçük harf ve Türkçe harf farkı yok sayılır): Tarih, Tutar, Gönderen, Açıklama, Referans; isteğe bağlı Saat.
/// Tarih <c>gg.aa.yyyy</c>, <c>gg/aa/yyyy</c> veya <c>yyyy-aa-gg</c> (saat aynı alanda "SS:dd" olarak da gelebilir; Europe/Istanbul).
/// Tutar Türkçe biçimde (<c>1.250,00</c>, <c>1250,5</c>, <c>1250</c>); eksi işaretli veya parantezli tutar giden hareket sayılır.
/// Banka seçilince sütun adları ve biçim o bankanın dökümüne uyarlanır.
/// </summary>
internal static class BankStatementCsv
{
    public const string DateColumn = "Tarih";
    public const string AmountColumn = "Tutar";
    public const string SenderColumn = "Gönderen";
    public const string DescriptionColumn = "Açıklama";
    public const string ReferenceColumn = "Referans";
    public const string TimeColumn = "Saat";

    private static readonly string[] Required = [DateColumn, AmountColumn, SenderColumn, ReferenceColumn];
    private static readonly string[] DatePatterns = ["dd.MM.uuuu", "d.M.uuuu", "dd/MM/uuuu", "d/M/uuuu", "uuuu-MM-dd"];
    private static readonly string[] TimePatterns = ["HH:mm", "HH:mm:ss", "H:mm"];

    /// <summary>Dosyayı ayrıştırır; başlık eksik veya okunamıyorsa null (dosya biçimi geçersiz).</summary>
    public static Parsed? Parse(string text, DateTimeZone zone)
    {
        ArgumentNullException.ThrowIfNull(text);
        var lines = SplitRecords(text.TrimStart('﻿'));
        if (lines.Count == 0)
        {
            return null;
        }

        var delimiter = lines[0].Count(c => c == ';') >= lines[0].Count(c => c == ',') ? ';' : ',';
        var header = SplitFields(lines[0], delimiter);
        var index = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var i = 0; i < header.Count; i++)
        {
            index.TryAdd(Key(header[i]), i);
        }

        if (Required.Any(c => !index.ContainsKey(Key(c))))
        {
            return null;
        }

        var rows = new List<Row>();
        var errors = new List<RowError>();
        for (var line = 1; line < lines.Count; line++)
        {
            if (string.IsNullOrWhiteSpace(lines[line]))
            {
                continue;
            }

            var fields = SplitFields(lines[line], delimiter);
            var rowNumber = line + 1;
            string? Field(string column) =>
                index.TryGetValue(Key(column), out var at) && at < fields.Count && fields[at].Trim() is { Length: > 0 } value ? value : null;

            var error = ParseRow(rowNumber, Field, zone, out var row);
            if (error is not null)
            {
                errors.Add(error);
            }
            else
            {
                rows.Add(row!);
            }
        }

        return new Parsed(rows.Count + errors.Count, rows, errors);
    }

    /// <summary>Türkçe tutar metnini kuruşa çevirir (eksi = giden). Geçersizse null.</summary>
    public static long? ParseAmount(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var value = text.Replace("TL", string.Empty, StringComparison.OrdinalIgnoreCase).Replace("₺", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal).Trim();
        var negative = false;
        if (value.StartsWith('(') && value.EndsWith(')'))
        {
            negative = true;
            value = value[1..^1];
        }

        if (value.StartsWith('-') || value.EndsWith('-'))
        {
            negative = true;
            value = value.Trim('-');
        }
        else if (value.StartsWith('+'))
        {
            value = value[1..];
        }

        string whole;
        var fraction = string.Empty;
        var comma = value.LastIndexOf(',');
        if (comma >= 0)
        {
            whole = value[..comma];
            fraction = value[(comma + 1)..];
            if (!ValidThousands(whole, '.'))
            {
                return null;
            }

            whole = whole.Replace(".", string.Empty, StringComparison.Ordinal);
        }
        else if (value.Count(c => c == '.') == 1 && value.Length - value.IndexOf('.') - 1 is 1 or 2)
        {
            // Nokta ondalıklı (1250.50): bazı dökümler böyle verir; üç basamaklı grup binlik sayılır.
            whole = value[..value.IndexOf('.')];
            fraction = value[(value.IndexOf('.') + 1)..];
        }
        else
        {
            if (!ValidThousands(value, '.'))
            {
                return null;
            }

            whole = value.Replace(".", string.Empty, StringComparison.Ordinal);
        }

        if (whole.Length is 0 or > 13 || !whole.All(char.IsAsciiDigit) || fraction.Length > 2 || !fraction.All(char.IsAsciiDigit))
        {
            return null;
        }

        var minor = (long.Parse(whole, CultureInfo.InvariantCulture) * 100) + (fraction.Length == 0 ? 0 : int.Parse(fraction.PadRight(2, '0'), CultureInfo.InvariantCulture));
        return negative ? -minor : minor;
    }

    private static RowError? ParseRow(int rowNumber, Func<string, string?> field, DateTimeZone zone, out Row? row)
    {
        row = null;
        var when = ParseWhen(rowNumber, field, zone, out var date, out var received);
        if (when is not null)
        {
            return when;
        }

        var amountText = field(AmountColumn);
        var sender = field(SenderColumn);
        var reference = field(ReferenceColumn);
        long amount = 0;
        var error = amountText is null ? new RowError(rowNumber, AmountColumn, "required")
            : ParseAmount(amountText) is not { } parsed ? new RowError(rowNumber, AmountColumn, "invalid_format")
            : (amount = parsed) == 0 ? new RowError(rowNumber, AmountColumn, "out_of_range")
            : sender is null ? new RowError(rowNumber, SenderColumn, "required")
            : reference is null ? new RowError(rowNumber, ReferenceColumn, "required")
            : reference.Length > PlatformBankTransaction.BankReferenceMaxLength ? new RowError(rowNumber, ReferenceColumn, "out_of_range")
            : null;
        if (error is null)
        {
            row = new Row(rowNumber, date, received, amount, sender!, field(DescriptionColumn), reference!);
        }

        return error;
    }

    private static RowError? ParseWhen(int rowNumber, Func<string, string?> field, DateTimeZone zone, out LocalDate date, out Instant? received)
    {
        date = default;
        received = null;
        var dateText = field(DateColumn);
        if (dateText is null)
        {
            return new RowError(rowNumber, DateColumn, "required");
        }

        var dateParts = dateText.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var timeText = dateParts.Length == 2 ? dateParts[1] : field(TimeColumn);
        if (ParseDate(dateParts[0]) is not { } parsedDate)
        {
            return new RowError(rowNumber, DateColumn, "invalid_format");
        }

        date = parsedDate;
        if (timeText is null)
        {
            return null;
        }

        if (ParseTime(timeText) is not { } time)
        {
            return new RowError(rowNumber, dateParts.Length == 2 ? DateColumn : TimeColumn, "invalid_format");
        }

        received = parsedDate.At(time).InZoneLeniently(zone).ToInstant();
        return null;
    }

    private static LocalDate? ParseDate(string text)
    {
        foreach (var pattern in DatePatterns)
        {
            var result = LocalDatePattern.CreateWithInvariantCulture(pattern).Parse(text);
            if (result.Success)
            {
                return result.Value;
            }
        }

        return null;
    }

    private static LocalTime? ParseTime(string text)
    {
        foreach (var pattern in TimePatterns)
        {
            var result = LocalTimePattern.CreateWithInvariantCulture(pattern).Parse(text);
            if (result.Success)
            {
                return result.Value;
            }
        }

        return null;
    }

    // "1.250" ve "12.500.000" geçerli binlik; "1.25" (nokta + iki basamak, virgüllü tutarda) geçersiz.
    private static bool ValidThousands(string whole, char separator)
    {
        var groups = whole.Split(separator);
        return groups.Length == 1 || (groups[0].Length is >= 1 and <= 3 && groups.Skip(1).All(g => g.Length == 3));
    }

    // Başlık adı karşılaştırması: Türkçe harfler ASCII'ye, büyük/küçük harf ve boşluk yok sayılır.
    private static string Key(string header)
    {
        var folded = header.Trim().Replace('ı', 'i').Replace('İ', 'I').Normalize(NormalizationForm.FormD);
        return new string([.. folded.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant)]);
    }

    // Tırnak içindeki satır sonları kaydı bölmez.
    private static List<string> SplitRecords(string text)
    {
        var records = new List<string>();
        var current = new StringBuilder();
        var quoted = false;
        foreach (var c in text)
        {
            if (c == '"')
            {
                quoted = !quoted;
            }

            if (!quoted && (c == '\n' || c == '\r'))
            {
                if (c == '\n' || current.Length > 0)
                {
                    records.Add(current.ToString());
                }

                current.Clear();
                continue;
            }

            current.Append(c);
        }

        if (current.Length > 0)
        {
            records.Add(current.ToString());
        }

        // \r\n çiftinin ürettiği boş kayıtlar ayıklanır; başlıktan önceki boş satırlar atlanır.
        var start = records.FindIndex(r => !string.IsNullOrWhiteSpace(r));
        return start < 0 ? [] : records.Skip(start).ToList();
    }

    private static List<string> SplitFields(string line, char delimiter)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    quoted = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c == '"')
            {
                quoted = true;
            }
            else if (c == delimiter)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }

    /// <summary>Ayrıştırma sonucu.</summary>
    /// <param name="TotalRows">Başlık dışı dolu satır.</param>
    /// <param name="Rows">Geçerli satırlar (giden tutarlar dahil, eksi işaretli).</param>
    /// <param name="Errors">Hatalı satırlar.</param>
    public sealed record Parsed(int TotalRows, IReadOnlyList<Row> Rows, IReadOnlyList<RowError> Errors);

    /// <summary>Geçerli satır.</summary>
    public sealed record Row(int RowNumber, LocalDate ValueDate, Instant? ReceivedUtc, long AmountMinor, string SenderName, string? Description, string BankReference);

    /// <summary>Hatalı satır (sözleşme BankTransactionImportResult.errors).</summary>
    public sealed record RowError(int RowNumber, string Column, string Code);
}
