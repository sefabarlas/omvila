namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Kurulum adımları (sözleşme OnboardingChecklist.steps[].key; dilim-01 § Alan kuralları).</summary>
public enum OnboardingStep
{
    /// <summary>Kayıtla tamamlanır.</summary>
    BusinessCreated,

    /// <summary>Şubede en az bir gün aralığı var.</summary>
    WorkingHours,

    /// <summary>En az bir aktif hizmet.</summary>
    Services,

    /// <summary>Sahip dışında personel veya "tek başıma çalışıyorum".</summary>
    Staff,

    /// <summary>Bağlantı kopyalandı veya QR indirildi.</summary>
    ShareBookingLink,
}
