namespace Omvila.Modules.Identity.Security;

/// <summary>Kayıt girdisi (uçta biçim denetiminden geçmiştir).</summary>
internal sealed record SignupCommand(string SignupToken, string BusinessName, string IndustryCode, string OwnerFullName, string? IpAddress);
