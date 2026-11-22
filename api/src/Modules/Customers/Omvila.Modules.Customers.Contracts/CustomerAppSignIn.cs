namespace Omvila.Modules.Customers.Contracts;

/// <summary>Uygulama girişinin sonucu.</summary>
/// <param name="CustomerId">Global kişi (me jetonunun sub iddiası).</param>
/// <param name="IsNewAccount">Uygulama hesabı bu girişle açıldı.</param>
public sealed record CustomerAppSignIn(Guid CustomerId, bool IsNewAccount);
