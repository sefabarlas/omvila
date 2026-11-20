namespace Omvila.Modules.Catalog.Features.Services;

/// <summary>Sözleşme ServiceStage.</summary>
internal sealed record StageDto(string? Name, int? DurationMinutes, bool? StaffBusy);
