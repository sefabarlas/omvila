namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Denetim kaydı girdisi (alan-modeli.md § AuditLog). <see cref="Changes"/> düz anahtar/değer çiftleridir ve kişisel veri
/// (ad, telefon, not, IBAN) taşımaz; sayılar, kimlikler ve kod değerleri yazılır.
/// </summary>
/// <param name="BrandId">Marka; platform geneli kayıtta null.</param>
/// <param name="ActorType">İşlemi yapan türü.</param>
/// <param name="ActorId">İşlemi yapan kullanıcı (varsa).</param>
/// <param name="Action">Eylem.</param>
/// <param name="EntityType">Varlık adı (ör. <c>BrandCustomer</c>).</param>
/// <param name="EntityId">Varlık kimliği (varsa).</param>
/// <param name="Changes">Ayrıntı (kişisel veri yok).</param>
/// <param name="IpAddress">İstemci IP adresi (varsa).</param>
/// <param name="UserAgent">İstemci (varsa).</param>
public sealed record AuditEntry(
    Guid? BrandId,
    AuditActorType ActorType,
    Guid? ActorId,
    AuditAction Action,
    string EntityType,
    Guid? EntityId,
    IReadOnlyDictionary<string, object?>? Changes = null,
    string? IpAddress = null,
    string? UserAgent = null);
