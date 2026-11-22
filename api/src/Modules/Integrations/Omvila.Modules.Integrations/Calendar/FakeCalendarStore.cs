using System.Collections.Concurrent;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// Sahte takvim sağlayıcısının bellek içi deposu (süreç başına tek). Testler ve geliştirme olayları buradan okur; bağlantı
/// kopmasını sınamak için jeton iptal edilebilir (<see cref="RevokeAll"/>).
/// </summary>
internal sealed class FakeCalendarStore
{
    private readonly ConcurrentDictionary<string, CalendarEventData> _events = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, byte> _revoked = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, CalendarEventData> Events => _events;

    public bool IsRevoked(string token) => _revoked.ContainsKey(token) || _revoked.ContainsKey("*");

    /// <summary>Verilen jeton (erişim veya yenileme) geçersiz olur.</summary>
    public void Revoke(string token) => _revoked[token] = 0;

    /// <summary>Bütün jetonlar geçersiz olur (kullanıcı Google'da erişimi kaldırdı).</summary>
    public void RevokeAll() => _revoked["*"] = 0;

    public void Reset()
    {
        _events.Clear();
        _revoked.Clear();
    }

    internal void Put(string eventId, CalendarEventData data) => _events[eventId] = data;

    internal void Remove(string eventId) => _events.TryRemove(eventId, out _);
}
