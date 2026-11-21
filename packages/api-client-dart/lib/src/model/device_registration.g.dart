// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'device_registration.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DeviceRegistrationCWProxy {
  DeviceRegistration platform(DevicePlatform platform);

  DeviceRegistration pushToken(String pushToken);

  DeviceRegistration appVersion(String appVersion);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DeviceRegistration(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DeviceRegistration(...).copyWith(id: 12, name: "My name")
  /// ````
  DeviceRegistration call({
    DevicePlatform platform,
    String pushToken,
    String appVersion,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDeviceRegistration.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDeviceRegistration.copyWith.fieldName(...)`
class _$DeviceRegistrationCWProxyImpl implements _$DeviceRegistrationCWProxy {
  const _$DeviceRegistrationCWProxyImpl(this._value);

  final DeviceRegistration _value;

  @override
  DeviceRegistration platform(DevicePlatform platform) =>
      this(platform: platform);

  @override
  DeviceRegistration pushToken(String pushToken) => this(pushToken: pushToken);

  @override
  DeviceRegistration appVersion(String appVersion) =>
      this(appVersion: appVersion);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DeviceRegistration(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DeviceRegistration(...).copyWith(id: 12, name: "My name")
  /// ````
  DeviceRegistration call({
    Object? platform = const $CopyWithPlaceholder(),
    Object? pushToken = const $CopyWithPlaceholder(),
    Object? appVersion = const $CopyWithPlaceholder(),
  }) {
    return DeviceRegistration(
      platform: platform == const $CopyWithPlaceholder()
          ? _value.platform
          // ignore: cast_nullable_to_non_nullable
          : platform as DevicePlatform,
      pushToken: pushToken == const $CopyWithPlaceholder()
          ? _value.pushToken
          // ignore: cast_nullable_to_non_nullable
          : pushToken as String,
      appVersion: appVersion == const $CopyWithPlaceholder()
          ? _value.appVersion
          // ignore: cast_nullable_to_non_nullable
          : appVersion as String,
    );
  }
}

extension $DeviceRegistrationCopyWith on DeviceRegistration {
  /// Returns a callable class that can be used as follows: `instanceOfDeviceRegistration.copyWith(...)` or like so:`instanceOfDeviceRegistration.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DeviceRegistrationCWProxy get copyWith =>
      _$DeviceRegistrationCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DeviceRegistration _$DeviceRegistrationFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DeviceRegistration', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['platform', 'pushToken', 'appVersion'],
      );
      final val = DeviceRegistration(
        platform: $checkedConvert(
          'platform',
          (v) => $enumDecode(
            _$DevicePlatformEnumMap,
            v,
            unknownValue: DevicePlatform.unknownDefaultOpenApi,
          ),
        ),
        pushToken: $checkedConvert('pushToken', (v) => v as String),
        appVersion: $checkedConvert('appVersion', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$DeviceRegistrationToJson(DeviceRegistration instance) =>
    <String, dynamic>{
      'platform': _$DevicePlatformEnumMap[instance.platform]!,
      'pushToken': instance.pushToken,
      'appVersion': instance.appVersion,
    };

const _$DevicePlatformEnumMap = {
  DevicePlatform.iOS: 'iOS',
  DevicePlatform.android: 'Android',
  DevicePlatform.unknownDefaultOpenApi: 'unknown_default_open_api',
};
