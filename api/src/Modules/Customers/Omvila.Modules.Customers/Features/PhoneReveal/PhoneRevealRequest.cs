namespace Omvila.Modules.Customers.Features.PhoneReveal;

/// <summary>Sözleşme PhoneRevealRequest: "Ara" (Call) veya "SMS" (Sms); amaç denetim kaydına yazılır.</summary>
internal sealed record PhoneRevealRequest(string? Purpose);
