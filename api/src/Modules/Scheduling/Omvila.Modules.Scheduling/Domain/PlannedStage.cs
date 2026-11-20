namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Hizmet aşaması; bekleme aşamasında <see cref="StaffBusy"/> false.</summary>
internal sealed record PlannedStage(string Name, int DurationMinutes, bool StaffBusy);
