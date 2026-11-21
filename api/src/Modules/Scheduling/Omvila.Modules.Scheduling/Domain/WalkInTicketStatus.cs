namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Kapıda sıra bileti durumu (durum-makinesi.md § 4).</summary>
internal enum WalkInTicketStatus
{
    /// <summary>Sırada.</summary>
    Waiting,

    /// <summary>Çağrıldı (bileti Called yapan uç v1'de yok; W5).</summary>
    Called,

    /// <summary>Randevuya çevrildi ("Başlat").</summary>
    Served,

    /// <summary>"Sıradan çıkar".</summary>
    Left,
}
