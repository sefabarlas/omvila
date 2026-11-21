using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

internal sealed record ReviewTopStaff(Guid StaffId, string FullName, double AverageRating, int Count);
