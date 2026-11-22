import 'package:flutter/foundation.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';

/// Push jetonunu kaydeder (meRegisterDevice).
// Tek işlevli arayüz bilerek: testte sahtesi verilir.
// ignore: one_member_abstracts
abstract interface class DeviceRepository {
  /// Jetonu müşteri hesabına bağlar; aynı jeton güncellenir.
  Future<void> register(String pushToken);
}

/// Üretilmiş istemciyle.
final class ApiDeviceRepository implements DeviceRepository {
  /// [api] müşteri jetonuyla.
  const ApiDeviceRepository(this.api);

  /// API.
  final OmvilaApi api;

  static const _appVersion = String.fromEnvironment(
    'APP_VERSION',
    defaultValue: '0.1.0',
  );

  @override
  Future<void> register(String pushToken) => apiCall(
    () => api.getMeNotificationsApi().meRegisterDevice(
      deviceRegistration: DeviceRegistration(
        platform: defaultTargetPlatform == TargetPlatform.iOS
            ? DevicePlatform.iOS
            : DevicePlatform.android,
        pushToken: pushToken,
        appVersion: _appVersion,
      ),
    ),
  );
}
