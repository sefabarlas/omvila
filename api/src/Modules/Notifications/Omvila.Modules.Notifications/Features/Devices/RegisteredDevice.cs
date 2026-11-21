using NodaTime;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Features.Devices;

/// <summary>Sözleşme RegisteredDevice.</summary>
/// <param name="DeviceId">Cihaz kaydı kimliği.</param>
/// <param name="Platform">Platform.</param>
/// <param name="LastSeenUtc">Son kayıt/yenileme anı.</param>
internal sealed record RegisteredDevice(Guid DeviceId, DevicePlatform Platform, Instant LastSeenUtc);
