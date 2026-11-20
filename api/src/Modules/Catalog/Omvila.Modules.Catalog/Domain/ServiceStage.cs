namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Hizmet aşaması (alan-modeli.md § ServiceStage): "Uygulama 45 dk · Bekleme 40 dk · Yıkama ve fön 35 dk".
/// Bekleme aşamasında <see cref="StaffBusy"/> false: personel başka müşteriye müsait görünür.
/// </summary>
internal sealed record ServiceStage(string Name, int DurationMinutes, bool StaffBusy);
