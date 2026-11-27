using System.Globalization;
using System.Text;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>
/// Tek sayfalık, yalnız metin içeren PDF (fatura PDF'i; bilgi amaçlı, e-Arşiv değil). Kütüphane yerine küçük bir yazıcı (T-118):
/// QuestPDF topluluk lisansı yıllık gelir sınırına bağlı ve yerel Skia ikilileri gerektiriyor; MIT lisanslı PDFsharp ise Linux
/// kapsayıcısında Türkçe harfli bir TTF yazı tipinin depoya gömülmesini istiyor. Bir sayfa metin için PDF'in standart 14 yazı
/// tipinden Helvetica, WinAnsi kodlaması ve Türkçe harfler için Differences tablosu yeterlidir (ğ Ğ ş Ş ı İ glif adları
/// Helvetica'nın standart glif kümesindedir).
/// </summary>
internal sealed class SimplePdf
{
    private const double PageWidth = 595.28;
    private const double PageHeight = 841.89;
    private const double Margin = 56;

    // WinAnsi'de olmayan Türkçe harfler kullanılmayan kod noktalarına eşlenir (Differences).
    private static readonly Dictionary<char, byte> TurkishCodes = new()
    {
        ['ğ'] = 0x81,
        ['Ğ'] = 0x8D,
        ['ş'] = 0x8F,
        ['Ş'] = 0x90,
        ['ı'] = 0x9D,
        ['İ'] = 0x9E,
    };

    // WinAnsi'de olup Latin-1 aralığının dışında kalan noktalama (dönem aralığındaki "–", adlardaki kıvrık kesme işareti).
    private static readonly Dictionary<char, byte> WinAnsiPunctuation = new()
    {
        ['…'] = 0x85,
        ['‘'] = 0x91,
        ['’'] = 0x92,
        ['“'] = 0x93,
        ['”'] = 0x94,
        ['–'] = 0x96,
        ['—'] = 0x97,
    };

    private static readonly Encoding Latin1 = Encoding.Latin1;

    private readonly List<(double X, double Y, double Size, bool Bold, string Text)> _lines = [];
    private double _cursor = PageHeight - Margin;

    /// <summary>Soldan bir satır yazar ve imleci aşağı taşır.</summary>
    public SimplePdf Line(string text, double size = 11, bool bold = false, double spacingAfter = 6)
    {
        _cursor -= size;
        _lines.Add((Margin, _cursor, size, bold, text));
        _cursor -= spacingAfter;
        return this;
    }

    /// <summary>Etiket solda, değer sağ sütunda.</summary>
    public SimplePdf Row(string label, string value, bool bold = false)
    {
        const double size = 11;
        _cursor -= size;
        _lines.Add((Margin, _cursor, size, false, label));
        _lines.Add((Margin + 220, _cursor, size, bold, value));
        _cursor -= 8;
        return this;
    }

    public SimplePdf Space(double points)
    {
        _cursor -= points;
        return this;
    }

    public byte[] Build()
    {
        var content = new StringBuilder();
        foreach (var (x, y, size, bold, text) in _lines)
        {
            content.Append(CultureInfo.InvariantCulture, $"BT /{(bold ? "F2" : "F1")} {size:0.##} Tf {x:0.##} {y:0.##} Td (")
                .Append(Escape(text))
                .Append(") Tj ET\n");
        }

        var stream = Latin1.GetBytes(content.ToString());
        const string differences = "/Differences [129 /gbreve 141 /Gbreve 143 /scedilla 144 /Scedilla 157 /dotlessi 158 /Idotaccent]";
        var objects = new List<byte[]>
        {
            Ascii("<< /Type /Catalog /Pages 2 0 R >>"),
            Ascii("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"),
            Ascii(string.Create(
                CultureInfo.InvariantCulture,
                $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth:0.##} {PageHeight:0.##}] /Resources << /Font << /F1 4 0 R /F2 5 0 R >> >> /Contents 7 0 R >>")),
            Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding 6 0 R >>"),
            Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding 6 0 R >>"),
            Ascii($"<< /Type /Encoding /BaseEncoding /WinAnsiEncoding {differences} >>"),
            Concat(Ascii(string.Create(CultureInfo.InvariantCulture, $"<< /Length {stream.Length} >>\nstream\n")), stream, Ascii("\nendstream")),
        };

        using var output = new MemoryStream();
        Write(output, Ascii("%PDF-1.4\n"));
        var offsets = new List<long>();
        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(output.Position);
            Write(output, Ascii(string.Create(CultureInfo.InvariantCulture, $"{i + 1} 0 obj\n")));
            Write(output, objects[i]);
            Write(output, Ascii("\nendobj\n"));
        }

        var xref = output.Position;
        var table = new StringBuilder();
        table.Append(CultureInfo.InvariantCulture, $"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets)
        {
            table.Append(CultureInfo.InvariantCulture, $"{offset:D10} 00000 n \n");
        }

        table.Append(CultureInfo.InvariantCulture, $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        Write(output, Ascii(table.ToString()));
        return output.ToArray();
    }

    /// <summary>PDF dizesine çevirir: Türkçe harfler ve WinAnsi noktalaması eşlenen koda, WinAnsi dışı kalanlar "?"; ( ) \ kaçırılır.</summary>
    internal static string Escape(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            var code = TurkishCodes.TryGetValue(c, out var mapped) || WinAnsiPunctuation.TryGetValue(c, out mapped) ? (char)mapped : c is >= ' ' and <= 'ÿ' ? c : '?';
            if (code is '(' or ')' or '\\')
            {
                builder.Append('\\');
            }

            builder.Append(code);
        }

        return builder.ToString();
    }

    private static byte[] Ascii(string value) => Latin1.GetBytes(value);

    private static byte[] Concat(params byte[][] parts) => [.. parts.SelectMany(p => p)];

    private static void Write(Stream output, byte[] bytes) => output.Write(bytes);
}
