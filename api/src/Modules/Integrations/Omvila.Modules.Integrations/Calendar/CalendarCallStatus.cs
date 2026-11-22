using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Sağlayıcı çağrısının sonucu.</summary>
internal enum CalendarCallStatus
{
    Ok,
    Revoked,
    Transient,
}
