// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_otp_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOtpRequestCWProxy {
  AdminOtpRequest phone(String phone);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOtpRequest call({String phone});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOtpRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOtpRequest.copyWith.fieldName(...)`
class _$AdminOtpRequestCWProxyImpl implements _$AdminOtpRequestCWProxy {
  const _$AdminOtpRequestCWProxyImpl(this._value);

  final AdminOtpRequest _value;

  @override
  AdminOtpRequest phone(String phone) => this(phone: phone);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOtpRequest call({Object? phone = const $CopyWithPlaceholder()}) {
    return AdminOtpRequest(
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
    );
  }
}

extension $AdminOtpRequestCopyWith on AdminOtpRequest {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOtpRequest.copyWith(...)` or like so:`instanceOfAdminOtpRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOtpRequestCWProxy get copyWith => _$AdminOtpRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOtpRequest _$AdminOtpRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminOtpRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['phone']);
      final val = AdminOtpRequest(
        phone: $checkedConvert('phone', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminOtpRequestToJson(AdminOtpRequest instance) =>
    <String, dynamic>{'phone': instance.phone};
