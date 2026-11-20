// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'otp_verify_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$OtpVerifyRequestCWProxy {
  OtpVerifyRequest otpId(String otpId);

  OtpVerifyRequest code(String code);

  OtpVerifyRequest rememberDevice(bool? rememberDevice);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OtpVerifyRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OtpVerifyRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  OtpVerifyRequest call({String otpId, String code, bool? rememberDevice});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfOtpVerifyRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfOtpVerifyRequest.copyWith.fieldName(...)`
class _$OtpVerifyRequestCWProxyImpl implements _$OtpVerifyRequestCWProxy {
  const _$OtpVerifyRequestCWProxyImpl(this._value);

  final OtpVerifyRequest _value;

  @override
  OtpVerifyRequest otpId(String otpId) => this(otpId: otpId);

  @override
  OtpVerifyRequest code(String code) => this(code: code);

  @override
  OtpVerifyRequest rememberDevice(bool? rememberDevice) =>
      this(rememberDevice: rememberDevice);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OtpVerifyRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OtpVerifyRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  OtpVerifyRequest call({
    Object? otpId = const $CopyWithPlaceholder(),
    Object? code = const $CopyWithPlaceholder(),
    Object? rememberDevice = const $CopyWithPlaceholder(),
  }) {
    return OtpVerifyRequest(
      otpId: otpId == const $CopyWithPlaceholder()
          ? _value.otpId
          // ignore: cast_nullable_to_non_nullable
          : otpId as String,
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
      rememberDevice: rememberDevice == const $CopyWithPlaceholder()
          ? _value.rememberDevice
          // ignore: cast_nullable_to_non_nullable
          : rememberDevice as bool?,
    );
  }
}

extension $OtpVerifyRequestCopyWith on OtpVerifyRequest {
  /// Returns a callable class that can be used as follows: `instanceOfOtpVerifyRequest.copyWith(...)` or like so:`instanceOfOtpVerifyRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$OtpVerifyRequestCWProxy get copyWith => _$OtpVerifyRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OtpVerifyRequest _$OtpVerifyRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('OtpVerifyRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['otpId', 'code']);
      final val = OtpVerifyRequest(
        otpId: $checkedConvert('otpId', (v) => v as String),
        code: $checkedConvert('code', (v) => v as String),
        rememberDevice: $checkedConvert(
          'rememberDevice',
          (v) => v as bool? ?? false,
        ),
      );
      return val;
    });

Map<String, dynamic> _$OtpVerifyRequestToJson(OtpVerifyRequest instance) =>
    <String, dynamic>{
      'otpId': instance.otpId,
      'code': instance.code,
      'rememberDevice': ?instance.rememberDevice,
    };
