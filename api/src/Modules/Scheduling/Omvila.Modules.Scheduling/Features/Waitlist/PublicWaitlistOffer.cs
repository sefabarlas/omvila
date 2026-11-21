using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme PublicWaitlistOffer (BeklemeTeklif, BeklemeDoldu). <see cref="DepositAmount"/> kapora yoksa yazılmaz.</summary>
internal sealed record PublicWaitlistOffer(
    Guid Id,
    string Status,
    PublicWaitlistOffer.BrandView Brand,
    PublicWaitlistOffer.BranchView Branch,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? CustomerFirstName,
    Instant StartUtc,
    Instant EndUtc,
    string TimeZone,
    Instant HoldExpiresUtc,
    Instant GeneratedUtc,
    IReadOnlyList<PublicWaitlistOffer.ItemView> Items,
    Money Total,
    Money AmountDue,
    bool RequiresApproval,
    Money? DepositAmount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? AppointmentAccessCode,
    string WaitlistAccessCode,
    string Version)
{
    /// <summary>PublicWaitlistOffer.brand.</summary>
    internal sealed record BrandView(string Name, string Slug, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl);

    /// <summary>PublicWaitlistOffer.branch.</summary>
    internal sealed record BranchView(Guid Id, string Name);

    /// <summary>PublicWaitlistOffer.items[].</summary>
    internal sealed record ItemView(string ServiceName, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? StaffName, int DurationMinutes, Money Price);
}
