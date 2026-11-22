using NodaTime;

namespace Omvila.Modules.Integrations.Domain;

/// <summary>
/// Bir randevunun bir bağlantıdaki takvim olayı (alan-modeli.md § AppointmentCalendarSync). <see cref="ContentHash"/> son
/// yazılan içeriğin özetidir (saat, başlık değişince olay güncellenir); <see cref="EventStartUtc"/> randevu personelden
/// alınırsa olayın pencerede bulunup silinmesi için.
/// </summary>
internal sealed class AppointmentCalendarSync
{
    private AppointmentCalendarSync()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid CalendarLinkId { get; private set; }

    public Guid AppointmentId { get; private set; }

    public string? ExternalEventId { get; private set; }

    public CalendarSyncStatus SyncStatus { get; private set; }

    public string ContentHash { get; private set; } = string.Empty;

    public Instant EventStartUtc { get; private set; }

    public Instant LastSyncedUtc { get; private set; }

    public static AppointmentCalendarSync Create(Guid id, Guid brandId, Guid linkId, Guid appointmentId) => new()
    {
        Id = id,
        BrandId = brandId,
        CalendarLinkId = linkId,
        AppointmentId = appointmentId,
        SyncStatus = CalendarSyncStatus.Failed,
    };

    public bool NeedsWrite(string hash) => SyncStatus != CalendarSyncStatus.Synced || !string.Equals(ContentHash, hash, StringComparison.Ordinal);

    public void Synced(string eventId, string hash, Instant eventStart, Instant now)
    {
        ExternalEventId = eventId;
        ContentHash = hash;
        EventStartUtc = eventStart;
        SyncStatus = CalendarSyncStatus.Synced;
        LastSyncedUtc = now;
    }

    public void Deleted(Instant now)
    {
        SyncStatus = CalendarSyncStatus.Deleted;
        LastSyncedUtc = now;
    }

    public void Failed(Instant now)
    {
        SyncStatus = CalendarSyncStatus.Failed;
        LastSyncedUtc = now;
    }
}
