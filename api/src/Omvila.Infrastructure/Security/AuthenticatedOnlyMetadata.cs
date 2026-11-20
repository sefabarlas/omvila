namespace Omvila.Infrastructure.Security;

/// <summary>
/// Uç yalnız geçerli bir oturum ister, belirli bir izin istemez (ör. çıkış). İşaretsiz uç denetimi bunu kabul eder.
/// </summary>
public sealed record AuthenticatedOnlyMetadata(string Audience);
