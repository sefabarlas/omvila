using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriteri 8 (kuaför şablonlarından 6 hizmet → kategoriler ve hizmetler), S-24 (fiyat zorunlu, sıfır
// olamaz), sözleşme manageListServiceTemplates / manageCreateServicesFromTemplates ("kategoriler yoksa oluşturulur").
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ServiceTemplateTests(PostgresFixture db) : IAsyncLifetime
{
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);

        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task HairSalonList_Has18Templates_UnknownIndustryIs404()
    {
        var list = await _api.GetAsync("/api/v1/manage/industries/hair_salon/service-templates", _token);

        list.Status.ShouldBe(200);
        list.Body.GetProperty("items").GetArrayLength().ShouldBe(18, "tasarım: Kuaför hazır listesi 18 hizmet");
        list.Body.GetProperty("items")[0].GetProperty("name").GetString().ShouldBe("Kadın saç kesimi");
        (await _api.GetAsync("/api/v1/manage/industries/kuafor/service-templates", _token)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task SixTemplates_CreateCategoriesAndServices_AssignedToOwner()
    {
        var templates = (await _api.GetAsync("/api/v1/manage/industries/hair_salon/service-templates", _token)).Body.GetProperty("items")
            .EnumerateArray().Take(6).ToList();
        var items = templates.Select((t, i) => new
        {
            templateId = t.GetProperty("id").GetString(),
            price = new { amountMinor = 40000 + (i * 5000), currency = "TRY" },
            durationMinutes = i == 0 ? 50 : (int?)null,
        }).ToArray();

        var result = await PostAsync(new { items });

        result.Status.ShouldBe(201);
        var services = result.Body.GetProperty("items").EnumerateArray().ToList();
        services.Count.ShouldBe(6);
        services[0].GetProperty("durationMinutes").GetInt32().ShouldBe(50, "süre seçimde değiştirilebilir");
        services[1].GetProperty("durationMinutes").GetInt32().ShouldBe(templates[1].GetProperty("defaultDurationMinutes").GetInt32());
        services[0].GetProperty("staffIds").GetArrayLength().ShouldBe(1);
        var categories = (await _api.GetAsync("/api/v1/manage/service-categories", _token)).Body.GetProperty("items")
            .EnumerateArray().Select(c => c.GetProperty("name").GetString()).ToList();
        categories.ShouldBe(["Kesim", "Fön ve şekillendirme"]);

        var again = await PostAsync(new { items = items.Take(1).ToArray() });
        again.Status.ShouldBe(400);
        again.Body.GetProperty("errors").GetProperty("items[0].templateId")[0].GetString().ShouldBe("already_exists");
    }

    [Fact]
    public async Task MissingOrZeroPrice_Returns400()
    {
        var template = (await _api.GetAsync("/api/v1/manage/industries/barber/service-templates", _token)).Body.GetProperty("items")[0]
            .GetProperty("id").GetString();

        var result = await PostAsync(new
        {
            items = new object[]
            {
                new { templateId = template },
                new { templateId = template, price = new { amountMinor = 0, currency = "TRY" } },
            },
        });

        result.Status.ShouldBe(400);
        result.Body.GetProperty("errors").EnumerateObject().Select(p => p.Name).ShouldContain(n => n.StartsWith("items[", StringComparison.Ordinal));
        (await _api.GetAsync("/api/v1/manage/services", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
    }

    private Task<ManageApi.Response> PostAsync(object body) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services/from-templates", body, _token,
            new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" });
}
