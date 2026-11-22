using NodaTime;
using Omvila.Modules.Integrations.Domain;

namespace Omvila.Modules.Integrations.Features.GoogleCalendar;

/// <summary>Sözleşme GoogleCalendarConnection. Bağlantı yoksa NotConnected; TokenExpired "Google Takvim bağlantısı koptu" (Durumlar).</summary>
internal sealed record GoogleCalendarConnection(string Status, string? AccountEmailMasked, Instant? ConnectedUtc, Instant? LastSyncedUtc)
{
    public static GoogleCalendarConnection NotConnected { get; } = new("NotConnected", null, null, null);

    public static GoogleCalendarConnection From(CalendarLink? link) =>
        link is null ? NotConnected : new(link.Status.ToString(), link.AccountEmailMasked, link.ConnectedUtc, link.LastSyncedUtc);
}
