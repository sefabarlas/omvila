namespace Omvila.Modules.Identity.Features.PublicBookingAuth;

/// <summary>Sözleşme PublicOtpRequest.</summary>
internal sealed record PublicOtpRequest(string? Phone, Guid? BranchId);
