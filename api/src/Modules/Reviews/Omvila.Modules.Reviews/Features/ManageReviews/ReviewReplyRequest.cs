using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>Sözleşme ReviewReplyRequest.</summary>
internal sealed record ReviewReplyRequest(string? Text);
