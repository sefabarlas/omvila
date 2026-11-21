// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'registered_device.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$RegisteredDeviceCWProxy {
  RegisteredDevice deviceId(String deviceId);

  RegisteredDevice platform(DevicePlatform platform);

  RegisteredDevice lastSeenUtc(DateTime lastSeenUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RegisteredDevice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RegisteredDevice(...).copyWith(id: 12, name: "My name")
  /// ````
  RegisteredDevice call({
    String deviceId,
    DevicePlatform platform,
    DateTime lastSeenUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfRegisteredDevice.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfRegisteredDevice.copyWith.fieldName(...)`
class _$RegisteredDeviceCWProxyImpl implements _$RegisteredDeviceCWProxy {
  const _$RegisteredDeviceCWProxyImpl(this._value);

  final RegisteredDevice _value;

  @override
  RegisteredDevice deviceId(String deviceId) => this(deviceId: deviceId);

  @override
  RegisteredDevice platform(DevicePlatform platform) =>
      this(platform: platform);

  @override
  RegisteredDevice lastSeenUtc(DateTime lastSeenUtc) =>
      this(lastSeenUtc: lastSeenUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RegisteredDevice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RegisteredDevice(...).copyWith(id: 12, name: "My name")
  /// ````
  RegisteredDevice call({
    Object? deviceId = const $CopyWithPlaceholder(),
    Object? platform = const $CopyWithPlaceholder(),
    Object? lastSeenUtc = const $CopyWithPlaceholder(),
  }) {
    return RegisteredDevice(
      deviceId: deviceId == const $CopyWithPlaceholder()
          ? _value.deviceId
          // ignore: cast_nullable_to_non_nullable
          : deviceId as String,
      platform: platform == const $CopyWithPlaceholder()
          ? _value.platform
          // ignore: cast_nullable_to_non_nullable
          : platform as DevicePlatform,
      lastSeenUtc: lastSeenUtc == const $CopyWithPlaceholder()
          ? _value.lastSeenUtc
          // ignore: cast_nullable_to_non_nullable
          : lastSeenUtc as DateTime,
    );
  }
}

extension $RegisteredDeviceCopyWith on RegisteredDevice {
  /// Returns a callable class that can be used as follows: `instanceOfRegisteredDevice.copyWith(...)` or like so:`instanceOfRegisteredDevice.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$RegisteredDeviceCWProxy get copyWith => _$RegisteredDeviceCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RegisteredDevice _$RegisteredDeviceFromJson(Map<String, dynamic> json) =>
    $checkedCreate('RegisteredDevice', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['deviceId', 'platform', 'lastSeenUtc'],
      );
      final val = RegisteredDevice(
        deviceId: $checkedConvert('deviceId', (v) => v as String),
        platform: $checkedConvert(
          'platform',
          (v) => $enumDecode(
            _$DevicePlatformEnumMap,
            v,
            unknownValue: DevicePlatform.unknownDefaultOpenApi,
          ),
        ),
        lastSeenUtc: $checkedConvert(
          'lastSeenUtc',
          (v) => DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$RegisteredDeviceToJson(RegisteredDevice instance) =>
    <String, dynamic>{
      'deviceId': instance.deviceId,
      'platform': _$DevicePlatformEnumMap[instance.platform]!,
      'lastSeenUtc': instance.lastSeenUtc.toIso8601String(),
    };

const _$DevicePlatformEnumMap = {
  DevicePlatform.iOS: 'iOS',
  DevicePlatform.android: 'Android',
  DevicePlatform.unknownDefaultOpenApi: 'unknown_default_open_api',
};
