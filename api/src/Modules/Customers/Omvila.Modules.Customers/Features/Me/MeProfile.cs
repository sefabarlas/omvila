using System.Text.Json.Serialization;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeProfile.</summary>
/// <param name="Id">Customer kimliği.</param>
/// <param name="FullName">Ad soyad; henüz verilmediyse null.</param>
/// <param name="ReviewerDisplayName">Değerlendirmede görünen ad ("Merve Y."); ad yoksa null.</param>
/// <param name="Phone">Telefon (E.164; salt okunur).</param>
/// <param name="Email">E-posta.</param>
/// <param name="Language">Dil (v1'de yalnız tr).</param>
/// <param name="Version">Sürüm (ETag ile aynı).</param>
internal sealed record MeProfile(
    Guid Id,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? FullName,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? ReviewerDisplayName,
    string Phone,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Email,
    string Language,
    string Version)
{
    public static MeProfile From(Customer customer) => new(
        customer.Id,
        customer.HasName ? customer.FullName : null,
        customer.HasName ? ReviewerName(customer.FullName) : null,
        customer.PhoneE164!,
        customer.Email,
        Customer.DefaultLanguageCode,
        ETagVersion.Format(customer.Version));

    // "Merve Yıldız" → "Merve Y." (ilk ad + soyadın baş harfi; tek kelimelik ad olduğu gibi).
    private static string ReviewerName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length < 2 ? parts[0] : $"{parts[0]} {char.ToUpper(parts[^1][0], System.Globalization.CultureInfo.GetCultureInfo("tr-TR"))}.";
    }
}
