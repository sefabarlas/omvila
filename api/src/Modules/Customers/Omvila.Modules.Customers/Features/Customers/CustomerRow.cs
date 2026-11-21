using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Liste ve kart sorgusunun projeksiyonu (kart + global kişinin telefonu). Ad salonun kart adıdır.</summary>
internal sealed class CustomerRow
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string? PhoneE164 { get; init; }

    public int? BirthYear { get; init; }

    public int VisitCount { get; init; }

    public int NoShowCount { get; init; }

    public LocalDate? FirstVisitOn { get; init; }

    public LocalDate? LastVisitOn { get; init; }

    public long TotalSpentMinor { get; init; }

    public string? Notes { get; init; }

    // Yaklaşan randevu (nextAppointmentUtc) uçta IAppointmentQueries ile doldurulur (T-50).
    public CustomerSummaryResponse ToResponse(CustomerViewContext view)
    {
        var activity = new CustomerActivity(VisitCount, NoShowCount, FirstVisitOn, LastVisitOn);
        var phone = CustomerPhoneDisplay.For(PhoneE164, view.CanSeeFullPhone);
        var threshold = view.Settings.RiskyCustomerNoShowThreshold;
        return new CustomerSummaryResponse(
            Id,
            FullName,
            phone.Phone,
            phone.PhoneMasked,
            BirthYear,
            VisitCount,
            NoShowCount,
            LastVisitOn,
            NextAppointmentUtc: null,
            new Money(TotalSpentMinor, view.Currency),
            CustomerSegments.IsNew(activity, view.Today),
            CustomerSegments.IsNoShowRisk(activity, threshold),
            CustomerSegments.Tags(activity, view.Today, threshold),
            Notes);
    }
}
