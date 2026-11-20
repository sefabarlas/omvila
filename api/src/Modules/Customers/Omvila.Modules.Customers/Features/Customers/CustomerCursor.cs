using System.Buffers.Text;
using System.Text;
using NodaTime;
using NodaTime.Text;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>
/// Müşteri listesinin imleci (api-kurallari.md § Liste ve sayfalama). Sıra: son ziyaret yeniden eskiye (hiç gelmeyenler
/// sonda), eşitlikte kart kimliği büyükten küçüğe. İmleç son satırın (son ziyaret, kimlik) çiftidir; istemci için opaktır.
/// </summary>
internal readonly record struct CustomerCursor(LocalDate? LastVisitOn, Guid Id)
{
    private const char NoVisit = '-';

    public string Encode()
    {
        var day = LastVisitOn is { } d ? LocalDatePattern.Iso.Format(d) : NoVisit.ToString();
        return Base64Url.EncodeToString(Encoding.UTF8.GetBytes($"{day}|{Id:N}"));
    }

    public static bool TryDecode(string? text, out CustomerCursor cursor)
    {
        cursor = default;
        if (string.IsNullOrEmpty(text) || !Base64Url.IsValid(text))
        {
            return false;
        }

        var parts = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(text)).Split('|');
        if (parts.Length != 2 || !Guid.TryParseExact(parts[1], "N", out var id))
        {
            return false;
        }

        if (string.Equals(parts[0], NoVisit.ToString(), StringComparison.Ordinal))
        {
            cursor = new CustomerCursor(null, id);
            return true;
        }

        var parsed = LocalDatePattern.Iso.Parse(parts[0]);
        if (!parsed.Success)
        {
            return false;
        }

        cursor = new CustomerCursor(parsed.Value, id);
        return true;
    }
}
