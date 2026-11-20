using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain;

/// <summary>Panelden eklenen müşterinin doğrulanmış girdisi.</summary>
internal sealed record NewCustomerInput(PhoneNumber Phone, string FullName, int? BirthYear, string? Notes);
