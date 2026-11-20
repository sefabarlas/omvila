namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Erişim kodunun amacı (alan-modeli.md § AppointmentAccessCode): /r/, /b/, /d/.</summary>
internal enum AccessCodePurpose
{
    Manage,
    WaitlistOffer,
    Review,
}
