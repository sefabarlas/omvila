namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Hesap kipi (openapi manageSearchSlots açıklaması).</summary>
internal enum AvailabilityMode
{
    /// <summary>Rezervasyon sayfası: MinNotice/MaxAdvance penceresi, ızgara hizası, yalnız online açık personel ve hizmet.</summary>
    Public,

    /// <summary>Salon paneli: pencere yok, online kapalı personel ve hizmetler dahil, ızgara dışı başlangıç serbest.</summary>
    Panel,
}
