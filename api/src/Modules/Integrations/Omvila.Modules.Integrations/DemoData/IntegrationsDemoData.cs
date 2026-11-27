using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Integrations.Calendar;
using Omvila.Modules.Integrations.Domain;
using Omvila.Modules.Integrations.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Integrations.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-131) için Integrations yazma yolu: personelin sahte Google Takvim bağlantısı.
/// Yalnız sahte sağlayıcı (<c>Integrations:Calendar:Provider = Fake</c>) etkinken yazılır; jetonlar uydurmadır ve
/// <see cref="CalendarTokenProtector"/> ile şifrelenir. Çağıranın transaction'ında, markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class IntegrationsDemoData
{
    private static readonly Duration TokenLifetime = Duration.FromHours(1);

    private readonly IntegrationsDbContext _db;
    private readonly CalendarTokenProtector _protector;
    private readonly IIdGenerator _ids;
    private readonly CalendarOptions _options;

    internal IntegrationsDemoData(IntegrationsDbContext db, CalendarTokenProtector protector, IIdGenerator ids, IOptions<CalendarOptions> options)
    {
        _db = db;
        _protector = protector;
        _ids = ids;
        _options = options.Value;
    }

    /// <summary>Sahte sağlayıcı etkin mi (değilse bağlantı yazılmaz).</summary>
    public bool IsFakeProvider => string.Equals(_options.Provider, CalendarOptions.FakeProvider, StringComparison.Ordinal);

    /// <summary>Etkin bağlantı (callback sonrası gibi). Dönüş: yazıldı mı.</summary>
    public async Task<bool> ConnectAsync(Guid brandId, Guid staffId, Guid userId, string accountEmailMasked, Instant now, CancellationToken ct)
    {
        if (!IsFakeProvider)
        {
            return false;
        }

        var link = CalendarLink.Create(_ids.New(), brandId, staffId, userId);
        link.Connect(
            userId, _ids.New(), "primary", accountEmailMasked,
            _protector.Protect(link.Id, $"fake-access-demo-{link.Id:N}"), _protector.Protect(link.Id, $"fake-refresh-demo-{link.Id:N}"), now + TokenLifetime, now);
        _db.CalendarLinks.Add(link);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
