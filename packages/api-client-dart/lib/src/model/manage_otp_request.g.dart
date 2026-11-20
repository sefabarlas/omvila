// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_otp_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageOtpRequestCWProxy {
  ManageOtpRequest phone(String phone);

  ManageOtpRequest purpose(ManageOtpRequestPurposeEnum purpose);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOtpRequest call({String phone, ManageOtpRequestPurposeEnum purpose});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageOtpRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageOtpRequest.copyWith.fieldName(...)`
class _$ManageOtpRequestCWProxyImpl implements _$ManageOtpRequestCWProxy {
  const _$ManageOtpRequestCWProxyImpl(this._value);

  final ManageOtpRequest _value;

  @override
  ManageOtpRequest phone(String phone) => this(phone: phone);

  @override
  ManageOtpRequest purpose(ManageOtpRequestPurposeEnum purpose) =>
      this(purpose: purpose);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOtpRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOtpRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOtpRequest call({
    Object? phone = const $CopyWithPlaceholder(),
    Object? purpose = const $CopyWithPlaceholder(),
  }) {
    return ManageOtpRequest(
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
      purpose: purpose == const $CopyWithPlaceholder()
          ? _value.purpose
          // ignore: cast_nullable_to_non_nullable
          : purpose as ManageOtpRequestPurposeEnum,
    );
  }
}

extension $ManageOtpRequestCopyWith on ManageOtpRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageOtpRequest.copyWith(...)` or like so:`instanceOfManageOtpRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageOtpRequestCWProxy get copyWith => _$ManageOtpRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageOtpRequest _$ManageOtpRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageOtpRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['phone', 'purpose']);
      final val = ManageOtpRequest(
        phone: $checkedConvert('phone', (v) => v as String),
        purpose: $checkedConvert(
          'purpose',
          (v) => $enumDecode(
            _$ManageOtpRequestPurposeEnumEnumMap,
            v,
            unknownValue: ManageOtpRequestPurposeEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageOtpRequestToJson(ManageOtpRequest instance) =>
    <String, dynamic>{
      'phone': instance.phone,
      'purpose': _$ManageOtpRequestPurposeEnumEnumMap[instance.purpose]!,
    };

const _$ManageOtpRequestPurposeEnumEnumMap = {
  ManageOtpRequestPurposeEnum.login: 'Login',
  ManageOtpRequestPurposeEnum.signup: 'Signup',
  ManageOtpRequestPurposeEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
