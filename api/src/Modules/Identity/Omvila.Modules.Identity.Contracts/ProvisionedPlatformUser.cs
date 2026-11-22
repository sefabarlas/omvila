using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Contracts;

/// <summary>Oluşturulan veya güncellenen platform kullanıcısı.</summary>
/// <param name="UserId">Kullanıcı.</param>
/// <param name="Created">Yeni mi.</param>
public sealed record ProvisionedPlatformUser(Guid UserId, bool Created);
