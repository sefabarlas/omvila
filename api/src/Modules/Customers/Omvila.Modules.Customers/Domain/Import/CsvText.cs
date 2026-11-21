using System.Text;

namespace Omvila.Modules.Customers.Domain.Import;

/// <summary>
/// Küçük RFC 4180 CSV okuyucu/yazıcı (yeni bağımlılık yerine): tırnaklı alan, alan içinde ayraç, satır sonu ve "" kaçışı.
/// Ayraç başlık satırından seçilir: noktalı virgül virgülden çoksa ';', değilse ','.
/// </summary>
internal static class CsvText
{
    public static char DetectDelimiter(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        int semicolons = 0, commas = 0;
        var quoted = false;
        foreach (var ch in text)
        {
            if (ch == '"')
            {
                quoted = !quoted;
            }
            else if (!quoted && ch is '\n' or '\r')
            {
                break;
            }
            else if (!quoted && ch == ';')
            {
                semicolons++;
            }
            else if (!quoted && ch == ',')
            {
                commas++;
            }
        }

        return semicolons > commas ? ';' : ',';
    }

    /// <summary>Satırları ayrıştırır; her kayıt 1'den başlayan dosya satır numarasıyla döner. Tamamen boş satırlar atlanır.</summary>
    public static IEnumerable<(int LineNumber, IReadOnlyList<string> Fields)> Parse(string text, char delimiter)
    {
        ArgumentNullException.ThrowIfNull(text);
        var position = 0;
        var line = 1;
        while (position < text.Length)
        {
            var start = line;
            var fields = ReadRecord(text, delimiter, ref position, ref line);
            if (fields.Exists(f => f.Length > 0))
            {
                yield return (start, fields);
            }
        }
    }

    /// <summary>
    /// CSV alanı yazar: gerekiyorsa tırnaklar. Elektronik tabloda formül olarak çalışabilecek değerin (=, +, -, @ ile
    /// başlayan) başına ' eklenir (CSV enjeksiyonu).
    /// </summary>
    public static string Field(string? value, char delimiter)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var safe = value[0] is '=' or '+' or '-' or '@' or '\t' or '\r' ? "'" + value : value;
        return safe.IndexOfAny([delimiter, '"', '\n', '\r']) >= 0 ? $"\"{safe.Replace("\"", "\"\"", StringComparison.Ordinal)}\"" : safe;
    }

    // Bir kaydı (satır sonuna veya metnin sonuna kadar) okur; tırnak içindeki satır sonu alana aittir.
    private static List<string> ReadRecord(string text, char delimiter, ref int position, ref int line)
    {
        var fields = new List<string>();
        var field = new StringBuilder();
        var quoted = false;
        while (position < text.Length)
        {
            var ch = text[position++];
            if (quoted)
            {
                var escapedQuote = ch == '"' && position < text.Length && text[position] == '"';
                quoted = ch != '"' || escapedQuote;
                position += escapedQuote ? 1 : 0;
                line += ch == '\n' ? 1 : 0;
                if (ch != '"' || escapedQuote)
                {
                    field.Append(ch);
                }
            }
            else if (ch == '"' && field.Length == 0)
            {
                quoted = true;
            }
            else if (ch == delimiter)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else if (ch is '\r' or '\n')
            {
                position += ch == '\r' && position < text.Length && text[position] == '\n' ? 1 : 0;
                line++;
                break;
            }
            else
            {
                field.Append(ch);
            }
        }

        fields.Add(field.ToString());
        return fields;
    }
}
