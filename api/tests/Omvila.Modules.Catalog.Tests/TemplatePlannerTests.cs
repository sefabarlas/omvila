using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Tests;

// Kural: dilim-01 kabul kriteri 8 (Kuaför şablonlarından 6 hizmet seçilir → kategoriler ve hizmetler oluşur),
// acik-sorular.md S-24 (fiyat zorunlu)
public sealed class TemplatePlannerTests
{
    private static TemplateSelection T(int n, string category, string name, int minutes, long? price) =>
        new(Guid.Parse($"0193a8f0-0000-7000-8000-{n:D12}"), category, name, minutes, price is { } p ? Money.Try(p) : null, null);

    [Fact]
    public void SixTemplates_CreateMissingCategoriesOnce_AndAllServices()
    {
        TemplateSelection[] selected =
        [
            T(1, "Saç", "Saç kesimi", 45, 45_000),
            T(2, "Saç", "Fön", 30, 25_000),
            T(3, "Renk", "Boya", 120, 120_000),
            T(4, "Renk", "Röfle", 150, 160_000),
            T(5, "Bakım", "Keratin", 60, 180_000),
            T(6, "Tırnak", "Manikür", 40, 30_000),
        ];

        var plan = TemplatePlanner.Plan(selected, existingCategories: ["SAÇ"]).Value;

        plan.NewCategories.ShouldBe(["Renk", "Bakım", "Tırnak"], "Saç zaten var (Türkçe büyük/küçük harf duyarsız)");
        plan.Services.Count.ShouldBe(6);
        plan.Services[2].ShouldBe(new PlannedService("Renk", "Boya", 120, Money.Try(120_000)));
    }

    [Fact]
    public void MissingPrice_IsValidationFailed() =>
        TemplatePlanner.Plan([T(1, "Saç", "Saç kesimi", 45, null)], [])
            .Error!.FieldErrors["items[0].price"].ShouldBe(["required"]);

    [Fact]
    public void EmptySelection_IsValidationFailed() =>
        TemplatePlanner.Plan([], []).Error!.FieldErrors["items"].ShouldBe(["required"]);
}
