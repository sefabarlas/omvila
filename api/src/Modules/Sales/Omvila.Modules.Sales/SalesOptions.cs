namespace Omvila.Modules.Sales;

/// <summary>appsettings "Sales" bölümü: sistem işlerinin çalışma aralıkları (teknik ayar; ADR-0005).</summary>
public sealed class SalesOptions
{
    public const string Section = "Sales";

    /// <summary>
    /// ExpireCustomerPackages turu. Kural günlüktür (son kullanım günü şubenin yerel takvimiyle geçince); iş idempotent olduğundan
    /// saatlik tur, paketi her saat diliminde yerel gece yarısından en geç bu kadar sonra Expired yapar.
    /// </summary>
    public int ExpireCustomerPackagesIntervalMinutes { get; set; }

    internal bool IsValid() => ExpireCustomerPackagesIntervalMinutes > 0;
}
