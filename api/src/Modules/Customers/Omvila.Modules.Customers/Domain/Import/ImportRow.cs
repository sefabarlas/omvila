namespace Omvila.Modules.Customers.Domain.Import;

/// <summary>Geçerli satır. E-posta modelde saklanmaz (T-92), yalnız uzunluğu denetlenir.</summary>
internal sealed record ImportRow(int RowNumber, string PhoneE164, string FullName, string? Notes, int? BirthYear);
