namespace Omvila.Modules.Identity.Contracts;

/// <summary>Kullanıcı adı.</summary>
/// <param name="FullName">Ad (yoksa boş).</param>
/// <param name="PlatformRole">Platform kullanıcısında rol (PlatformAdmin, PlatformSupport, PlatformFinance); diğerlerinde null.</param>
public sealed record UserNameInfo(string FullName, string? PlatformRole);
