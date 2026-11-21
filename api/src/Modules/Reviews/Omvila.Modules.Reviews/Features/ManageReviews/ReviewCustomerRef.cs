using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>Sözleşme CustomerRef (panelde tam ad; telefon yazılmaz).</summary>
internal sealed record ReviewCustomerRef(Guid BrandCustomerId, string FullName);
