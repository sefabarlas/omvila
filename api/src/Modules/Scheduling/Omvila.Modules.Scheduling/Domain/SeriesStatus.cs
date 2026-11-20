namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Tekrarlayan randevu serisinin durumu (durum-makinesi.md § 10: Active ↔ Paused → Ended).</summary>
internal enum SeriesStatus
{
    Active,
    Paused,
    Ended,
}
