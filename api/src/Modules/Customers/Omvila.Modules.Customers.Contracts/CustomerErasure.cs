using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Anonimleştirme girdisi.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BrandCustomerId">Anonimleştirilen kart (kimliği değişmez).</param>
/// <param name="AnonymousCustomerId">Kartın yeni global kişisi (anonim kayıt).</param>
/// <param name="UserId">İşlemi yapan kullanıcı.</param>
/// <param name="Now">Şimdi.</param>
public sealed record CustomerErasure(Guid BrandId, Guid BrandCustomerId, Guid AnonymousCustomerId, Guid? UserId, Instant Now);
