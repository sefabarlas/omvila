// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_otp_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicOtpRequestCWProxy {
  PublicOtpRequest phone(String phone);

  PublicOtpRequest branchId(String branchId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicOtpRequest call({String phone, String branchId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicOtpRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicOtpRequest.copyWith.fieldName(...)`
class _$PublicOtpRequestCWProxyImpl implements _$PublicOtpRequestCWProxy {
  const _$PublicOtpRequestCWProxyImpl(this._value);

  final PublicOtpRequest _value;

  @override
  PublicOtpRequest phone(String phone) => this(phone: phone);

  @override
  PublicOtpRequest branchId(String branchId) => this(branchId: branchId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicOtpRequest call({
    Object? phone = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
  }) {
    return PublicOtpRequest(
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
    );
  }
}

extension $PublicOtpRequestCopyWith on PublicOtpRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPublicOtpRequest.copyWith(...)` or like so:`instanceOfPublicOtpRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicOtpRequestCWProxy get copyWith => _$PublicOtpRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicOtpRequest _$PublicOtpRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicOtpRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['phone', 'branchId']);
      final val = PublicOtpRequest(
        phone: $checkedConvert('phone', (v) => v as String),
        branchId: $checkedConvert('branchId', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$PublicOtpRequestToJson(PublicOtpRequest instance) =>
    <String, dynamic>{'phone': instance.phone, 'branchId': instance.branchId};
