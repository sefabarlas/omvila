namespace Omvila.Modules.Identity.Features.PublicBookingAuth;

/// <summary>Sözleşme BookingTokenResult. <see cref="KnownCustomerFirstName"/>: "Tekrar hoş geldiniz, Ayşe" (salonda kartı varsa).</summary>
internal sealed record BookingTokenResult(string BookingToken, int ExpiresInSeconds, string PhoneMasked, string? KnownCustomerFirstName);
