using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Uygulama hesabının telefonu ve profildeki adı.</summary>
/// <param name="Phone">Doğrulanmış telefon (giriş anahtarı).</param>
/// <param name="FullName">Profildeki ad; henüz verilmemişse null.</param>
public sealed record CustomerAppIdentity(PhoneNumber Phone, string? FullName);
