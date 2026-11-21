using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>Online randevu isteğinin bağlamı: şube (yoldan), doğrulanmış telefon (jetondan), istemci ve IP (rıza kaydı).</summary>
/// <param name="Scope">Şube ve markası.</param>
/// <param name="Phone">Rezervasyon jetonundaki doğrulanmış telefon.</param>
/// <param name="ClientApp">Rezervasyon sayfası veya müşteri uygulaması (X-Client).</param>
/// <param name="ConsentChannel">Rızanın alındığı kanal.</param>
/// <param name="IpAddress">İstemci IP'si (yalnız rıza kanıtı; günlüğe yazılmaz).</param>
internal sealed record PublicBookingRequest(PublicBranchScope Scope, PhoneNumber Phone, ClientApp ClientApp, ConsentChannel ConsentChannel, string? IpAddress);
