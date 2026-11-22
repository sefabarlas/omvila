using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme SubscriptionInvoice.</summary>
internal sealed record SubscriptionInvoiceView(
    Guid Id,
    string InvoiceNumber,
    LocalDate PeriodStart,
    LocalDate PeriodEnd,
    Money Amount,
    Money Tax,
    Money Total,
    SubscriptionInvoiceStatus Status,
    Instant IssuedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? PaidUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] SubscriptionPaymentMethod? PaidVia)
{
    public static SubscriptionInvoiceView From(SubscriptionInvoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        return new SubscriptionInvoiceView(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.PeriodStart,
            invoice.PeriodEnd,
            new Money(invoice.AmountMinor, invoice.Currency),
            new Money(invoice.TaxMinor, invoice.Currency),
            new Money(invoice.TotalMinor, invoice.Currency),
            invoice.Status,
            invoice.IssuedUtc,
            invoice.PaidUtc,
            invoice.PaidVia);
    }
}
