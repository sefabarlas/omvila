namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme ManageCancelRequest. offerToWaitlist (varsayılan true): boşalan saat bekleme listesine teklif edilsin mi (T-90).</summary>
internal sealed record ManageCancelRequest(string? Reason, string? Note, string? Scope, bool? NotifyCustomer, bool? OfferToWaitlist);
