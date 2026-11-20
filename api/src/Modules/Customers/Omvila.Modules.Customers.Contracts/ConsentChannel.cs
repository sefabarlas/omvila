namespace Omvila.Modules.Customers.Contracts;

/// <summary>Rızanın alındığı kanal.</summary>
public enum ConsentChannel
{
    /// <summary>Rezervasyon sayfası.</summary>
    Web,

    /// <summary>Müşteri uygulaması.</summary>
    CustomerApp,

    /// <summary>İşletme uygulaması.</summary>
    BusinessApp,

    /// <summary>Salon paneli.</summary>
    Panel,

    /// <summary>CSV içe aktarma.</summary>
    Import,
}
