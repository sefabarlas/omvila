namespace Omvila.Modules.Identity.Features.AdminAuth;

/// <summary>Sözleşme AdminSession.user.</summary>
internal sealed record AdminSessionUser(Guid Id, string FullName, string PhoneMasked);
