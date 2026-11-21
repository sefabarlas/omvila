namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme CustomerPackageItemView.</summary>
internal sealed record CustomerPackageItemView(Guid CustomerPackageItemId, Guid ServiceId, string ServiceName, int TotalSessions, int RemainingSessions);
