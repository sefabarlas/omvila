namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>"Selin Öztürk randevusu bitince eklenecek".</summary>
internal sealed record NextAccrual(Guid AppointmentId, string CustomerName);
