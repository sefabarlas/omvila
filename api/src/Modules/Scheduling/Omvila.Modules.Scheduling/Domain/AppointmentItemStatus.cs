namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Kalem durumu (durum-makinesi.md § 2). Kalemde NoShow yoktur.</summary>
internal enum AppointmentItemStatus
{
    Active,
    Completed,
    Cancelled,
}
