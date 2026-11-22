namespace Omvila.Modules.Integrations.Features.GoogleCalendar;

/// <summary>Sözleşme GoogleCalendarCallbackRequest.</summary>
internal sealed record GoogleCalendarCallbackRequest(string? Code, string? State);
