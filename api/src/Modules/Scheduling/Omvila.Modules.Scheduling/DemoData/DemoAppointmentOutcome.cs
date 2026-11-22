namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>Demo randevusunun varacağı durum (durum-makinesi.md).</summary>
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

    /// <summary>Rezervasyon sayfasından, kaporalı (Kapora bekleniyor, Awaiting); tutma süresi havale süresi, seed anından.</summary>
    DepositAwaiting,

    /// <summary>Kapora bekleniyor ve müşteri "Havaleyi yaptım" dedi (CustomerReported; panelde "Havale bildirildi").</summary>
    DepositReported,

    /// <summary>Salon "Kapora geldi" dedi (A3): Onaylandı, Sales'te tahsilat kaydı (Received).</summary>
    DepositReceived,

    /// <summary>Salon "Kaporasız onayla" dedi (A2, waiveDeposit): Onaylandı, kapora istenmiyor (Waived).</summary>
    DepositWaived,

    /// <summary>
    /// Geldi (A10) ve bitişinde "Tamamla ve ödeme al" (A11); paket kalemi verilmişse paketten düşülür. Tahsilat ve prim çağıranın
    /// (Sales demo verisi), ziyaret sayacı burada.
    /// </summary>
    Completed,
}
