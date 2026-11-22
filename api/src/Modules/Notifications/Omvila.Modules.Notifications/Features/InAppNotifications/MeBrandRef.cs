using System.Text.Json.Serialization;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>Sözleşme MeBrandRef.</summary>
/// <param name="Id">Marka.</param>
/// <param name="Name">Ad.</param>
/// <param name="Slug">Güncel bağlantı adı.</param>
/// <param name="LogoUrl">Logo.</param>
internal sealed record MeBrandRef(Guid Id, string Name, string Slug, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl);
