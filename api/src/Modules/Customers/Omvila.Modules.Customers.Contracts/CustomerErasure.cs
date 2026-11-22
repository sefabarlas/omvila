using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Anonimleştirme girdisi.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BrandCustomerId">Anonimleştirilen kart (kimliği değişmez).</param>
/// <param name="AnonymousCustomerId">Kartın yeni global kişisi (anonim kayıt).</param>
/// <param name="UserId">İşlemi yapan kullanıcı.</param>
/// <param name="Now">Şimdi.</param>
/// <param name="ByCustomer">
/// Müşteri kendi hesabını uygulamadan sildi (KK9): iptal müşteri iptali gibi duyurulur (salona staff.appointment_changed, boşalan saat
/// bekleme listesine) ve paketten düşülen seanslar geri döner. Salon panelinden silmede false.
/// </param>
public sealed record CustomerErasure(Guid BrandId, Guid BrandCustomerId, Guid AnonymousCustomerId, Guid? UserId, Instant Now, bool ByCustomer = false);
