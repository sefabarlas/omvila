using System.Globalization;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Hazır liste (dilim-01 kabul kriteri 8; sözleşme: "Kategoriler yoksa oluşturulur"). Kategori adı marka içinde
/// tektir; karşılaştırma Türkçe kurallarıyla büyük/küçük harf duyarsızdır. Fiyat zorunlu (acik-sorular.md S-24).
/// </summary>
internal static class TemplatePlanner
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public static Result<TemplatePlan> Plan(IReadOnlyList<TemplateSelection> selections, IReadOnlyCollection<string> existingCategories)
    {
        ArgumentNullException.ThrowIfNull(selections);
        ArgumentNullException.ThrowIfNull(existingCategories);
        if (selections.Count == 0)
        {
            return Error.Validation("items", "required");
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var comparer = StringComparer.Create(Turkish, ignoreCase: true);
        var known = new HashSet<string>(existingCategories, comparer);
        var newCategories = new List<string>();
        var services = new List<PlannedService>();
        for (var i = 0; i < selections.Count; i++)
        {
            var selection = selections[i];
            if (selection.Price is not { } price)
            {
                errors[$"items[{i}].price"] = ["required"];
                continue;
            }

            var duration = selection.DurationMinutes ?? selection.DefaultDurationMinutes;
            if (duration is < ServiceRules.MinDurationMinutes or > ServiceRules.MaxDurationMinutes)
            {
                errors[$"items[{i}].durationMinutes"] = ["out_of_range"];
                continue;
            }

            if (known.Add(selection.CategoryName))
            {
                newCategories.Add(selection.CategoryName);
            }

            services.Add(new PlannedService(selection.CategoryName, selection.Name, duration, price));
        }

        return errors.Count > 0 ? Error.Validation(errors) : new TemplatePlan(newCategories, services);
    }
}
