namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Serinin şablon kalemi (alan-modeli.md § AppointmentSeries: hizmet, personel, göreli başlangıç).</summary>
internal sealed record SeriesItemTemplate(Guid ServiceId, Guid? StaffId, int OffsetMinutes);
