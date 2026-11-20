namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Hizmet aşaması ("Uygulama 45 dk · Bekleme 40 dk · Yıkama ve fön 35 dk").</summary>
/// <param name="Name">Aşama adı.</param>
/// <param name="DurationMinutes">Süre (personel farkı uygulanmış).</param>
/// <param name="StaffBusy">Personel meşgul mü; bekleme aşamasında <c>false</c>.</param>
public sealed record ServiceOfferStage(string Name, int DurationMinutes, bool StaffBusy);
