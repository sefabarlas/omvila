namespace Omvila.Modules.Notifications.Features.Devices;

/// <summary>Sözleşme DeviceRegistration.</summary>
/// <param name="Platform">iOS veya Android.</param>
/// <param name="PushToken">FCM kayıt jetonu.</param>
/// <param name="AppVersion">Uygulama sürümü.</param>
internal sealed record DeviceRegistration(string? Platform, string? PushToken, string? AppVersion);
