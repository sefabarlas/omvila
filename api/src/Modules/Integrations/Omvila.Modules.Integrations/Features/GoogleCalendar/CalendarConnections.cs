using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Integrations.Calendar;
using Omvila.Modules.Integrations.Domain;
using Omvila.Modules.Integrations.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Integrations.Features.GoogleCalendar;

/// <summary>
/// Google Takvim bağlantısı (K7; sözleşme manage-integrations): bağlantı adresi (imzalı state), bağlantıyı tamamlama, kesme.
/// Yeniden bağlanınca bağlantının aktarım kayıtları sıfırlanır: gelecekteki randevular yeni hesaba baştan yazılır, eski hesaptaki
/// olaylar silinmez (sözleşme). Kesince jetonlar silinir ve sağlayıcıdaki erişim iptal edilir.
/// </summary>
internal sealed class CalendarConnections(
    IntegrationsDbContext db, ICalendarProvider provider, CalendarTokenProtector protector, IOptions<CalendarOptions> options, IIdGenerator ids, IClock clock)
{
    public Task<CalendarLink?> FindAsync(CalendarActor actor, CancellationToken ct) =>
        db.CalendarLinks.AsNoTracking().SingleOrDefaultAsync(l => l.BrandId == actor.BrandId && l.StaffId == actor.StaffId, ct);

    public Result<GoogleCalendarConnectUrl> CreateConnectUrl(CalendarActor actor, string? client)
    {
        var redirect = RedirectUri(client);
        if (client is not (null or "web" or "mobile"))
        {
            return Error.Validation("client", "invalid_format");
        }

        if (!provider.IsEnabled || redirect is null)
        {
            // Sağlayıcı kapalı (Google istemcisi yapılandırılmamış): bağlanılamaz (T-121).
            return Error.Unavailable();
        }

        var expires = clock.GetCurrentInstant() + Duration.FromMinutes(options.Value.StateMinutes);
        var state = new CalendarConnectState(actor.BrandId, actor.UserId, actor.StaffId, ids.New(), expires, string.Equals(client, "mobile", StringComparison.Ordinal))
            .Sign(protector.StateKey);
        return new GoogleCalendarConnectUrl(provider.BuildAuthorizationUrl(state, redirect), state, expires);
    }

    // Kural: sözleşme manageCompleteGoogleCalendarConnect (state 10 dk, kullanıcıya bağlı, tek kullanımlık)
    public async Task<Result<CalendarLink>> CompleteAsync(CalendarActor actor, GoogleCalendarCallbackRequest body, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var state = CalendarConnectState.Read(body.State, protector.StateKey, now);
        if (state is null || state.BrandId != actor.BrandId || state.UserId != actor.UserId || state.StaffId != actor.StaffId)
        {
            return Error.Validation("state", "invalid_format");
        }

        var link = await db.CalendarLinks.SingleOrDefaultAsync(l => l.BrandId == actor.BrandId && l.StaffId == actor.StaffId, ct);
        if (link?.ConnectNonce == state.Nonce)
        {
            return Error.Validation("state", "invalid_format");
        }

        var redirect = RedirectUri(state.Mobile ? "mobile" : null);
        var authorization = string.IsNullOrWhiteSpace(body.Code) || !provider.IsEnabled || redirect is null
            ? null
            : await provider.ExchangeCodeAsync(body.Code, redirect, ct);
        if (authorization is null)
        {
            return Error.Validation("code", "invalid_format");
        }

        if (link is null)
        {
            link = CalendarLink.Create(ids.New(), actor.BrandId, actor.StaffId, actor.UserId);
            db.CalendarLinks.Add(link);
        }
        else
        {
            await db.CalendarSyncs.Where(s => s.CalendarLinkId == link.Id).ExecuteDeleteAsync(ct);
        }

        link.Connect(
            actor.UserId,
            state.Nonce,
            authorization.CalendarId,
            authorization.AccountEmail is { } email ? MaskEmail(email) : null,
            protector.Protect(link.Id, authorization.AccessToken),
            protector.Protect(link.Id, authorization.RefreshToken),
            authorization.ExpiresUtc,
            now);
        await db.SaveChangesAsync(ct);
        return link;
    }

    public async Task DisconnectAsync(CalendarActor actor, CancellationToken ct)
    {
        var link = await db.CalendarLinks.SingleOrDefaultAsync(l => l.BrandId == actor.BrandId && l.StaffId == actor.StaffId, ct);
        if (link is null || link.Status == CalendarLinkStatus.Revoked)
        {
            return;
        }

        var refresh = link.RefreshTokenEncrypted is { } encrypted ? protector.Unprotect(link.Id, encrypted) : null;
        link.Revoke(clock.GetCurrentInstant());
        await db.CalendarSyncs.Where(s => s.CalendarLinkId == link.Id).ExecuteDeleteAsync(ct);
        await db.SaveChangesAsync(ct);
        if (refresh is not null)
        {
            await provider.RevokeAsync(refresh, ct);
        }
    }

    internal static string MaskEmail(string email)
    {
        var at = email.IndexOf('@', StringComparison.Ordinal);
        return at <= 0 ? "***" : $"{email[..Math.Min(2, at)]}***{email[at..]}";
    }

    // Geri dönüş adresi kodun alındığı adresle aynı olmalı (OAuth); istemci seçimi state'te taşınır. Mobil adres yoksa web.
    private Uri? RedirectUri(string? client) =>
        string.Equals(client, "mobile", StringComparison.Ordinal) ? options.Value.RedirectUris.Mobile ?? options.Value.RedirectUris.Web : options.Value.RedirectUris.Web;
}
