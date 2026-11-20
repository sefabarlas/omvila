namespace Omvila.Modules.Scheduling.Domain;

/// <summary>İptal eden taraf (alan-modeli.md § Appointment.CancelledBy).</summary>
internal enum CancelledBy
{
    Customer,
    Salon,
    System,
}
