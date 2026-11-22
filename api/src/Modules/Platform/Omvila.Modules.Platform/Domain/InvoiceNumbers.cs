using System.Globalization;

namespace Omvila.Modules.Platform.Domain;

/// <summary>Fatura numarası biçimi <c>OMV-2026-000184</c> (alan-modeli.md § SubscriptionInvoice).</summary>
internal static class InvoiceNumbers
{
    public const string Prefix = "OMV";

    public static string Format(int year, long sequence) =>
        string.Create(CultureInfo.InvariantCulture, $"{Prefix}-{year:D4}-{sequence:D6}");
}
