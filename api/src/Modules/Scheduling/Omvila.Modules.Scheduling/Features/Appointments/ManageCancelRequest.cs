namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme ManageCancelRequest. offerToWaitlist bekleme listesiyle (Dilim 5) gelir; şimdilik okunmaz.</summary>
internal sealed record ManageCancelRequest(string? Reason, string? Note, string? Scope, bool? NotifyCustomer, bool? OfferToWaitlist);
