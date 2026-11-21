// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_otp_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeOtpRequestCWProxy {
  MeOtpRequest phone(String phone);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeOtpRequest call({String phone});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeOtpRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeOtpRequest.copyWith.fieldName(...)`
class _$MeOtpRequestCWProxyImpl implements _$MeOtpRequestCWProxy {
  const _$MeOtpRequestCWProxyImpl(this._value);

  final MeOtpRequest _value;

  @override
  MeOtpRequest phone(String phone) => this(phone: phone);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeOtpRequest call({Object? phone = const $CopyWithPlaceholder()}) {
    return MeOtpRequest(
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
    );
  }
}

extension $MeOtpRequestCopyWith on MeOtpRequest {
  /// Returns a callable class that can be used as follows: `instanceOfMeOtpRequest.copyWith(...)` or like so:`instanceOfMeOtpRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeOtpRequestCWProxy get copyWith => _$MeOtpRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeOtpRequest _$MeOtpRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeOtpRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['phone']);
      final val = MeOtpRequest(
        phone: $checkedConvert('phone', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$MeOtpRequestToJson(MeOtpRequest instance) =>
    <String, dynamic>{'phone': instance.phone};
