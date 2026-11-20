namespace Omvila.Infrastructure.Http;

/// <summary>Benzersiz indeks adı → iş hatası kodu (ör. ix_brand_customers_brand_id_phone → phone_already_exists).</summary>
internal sealed record UniqueConstraintError(string ConstraintName, string Code);
