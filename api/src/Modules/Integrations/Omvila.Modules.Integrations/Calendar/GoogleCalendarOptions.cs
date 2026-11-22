namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Google OAuth istemcisi (mağaza hazırlığıyla açılır; K7). Gizli değer ortamdan.</summary>
internal sealed class GoogleCalendarOptions
{
    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
}
