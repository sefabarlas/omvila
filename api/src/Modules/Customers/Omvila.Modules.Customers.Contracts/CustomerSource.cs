namespace Omvila.Modules.Customers.Contracts;

/// <summary>Müşterinin markaya ilk geldiği kanal ("İlk kanal"; alan-modeli.md § BrandCustomer.Source).</summary>
public enum CustomerSource
{
    /// <summary>Rezervasyon sayfası veya müşteri uygulaması.</summary>
    Online,

    /// <summary>Panelden "Müşteri ekle".</summary>
    Panel,

    /// <summary>Telefonla gelen randevu.</summary>
    Phone,

    /// <summary>Kapıdan gelen müşteri.</summary>
    WalkIn,

    /// <summary>CSV içe aktarma.</summary>
    Import,
}
