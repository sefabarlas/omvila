namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// Kullanıcı adları (platform paneli denetim kaydı "Kim" sütunu ve "Marka veya kişi ara"; T-124). Platform okuma servisidir
/// (ADR-0006): yalnız admin yüzeyinin platform bağlamında çağrılır. Telefon dönmez.
/// </summary>
public interface IUserNameDirectory
{
    /// <summary>Kullanıcıların adı ve (platform kullanıcısında) rolü; bulunmayan kimlik sözlükte yoktur.</summary>
    /// <param name="userIds">Kullanıcılar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, UserNameInfo>> GetAsync(IReadOnlyCollection<Guid> userIds, CancellationToken ct);

    /// <summary>Adında <paramref name="query"/> geçen kullanıcılar (büyük/küçük harf farkı yok sayılır; en fazla 200).</summary>
    /// <param name="query">Arama metni.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<Guid>> FindByNameAsync(string query, CancellationToken ct);
}
