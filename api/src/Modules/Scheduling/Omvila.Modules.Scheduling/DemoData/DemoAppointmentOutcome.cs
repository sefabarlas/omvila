namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>Demo randevusunun varacağı durum (durum-makinesi.md; <c>Completed</c> adisyonla gelir, Dilim 6).</summary>
public enum DemoAppointmentOutcome
{
    /// <summary>Telefonla, onaylı.</summary>
    Confirmed,

    /// <summary>Rezervasyon sayfasından, onay gerektiren hizmetle (A1); tutma süresi markanın onay süresi.</summary>
    PendingApproval,

    /// <summary>Geldi (A10), başlangıç anında.</summary>
    CheckedIn,

    /// <summary>Gelmedi (A8), gelmeme süresi dolunca; müşterinin gelmeme sayacı artar.</summary>
    NoShow,

    /// <summary>Salon iptali (A7, "Müşteri istedi").</summary>
    CancelledBySalon,

    /// <summary>Müşteri iptali (A6, "Planım değişti"); iptal sınırı dışında olmalı. İptal sayacı artar.</summary>
    CancelledByCustomer,
}
