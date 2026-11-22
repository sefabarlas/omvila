// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'phone_reveal_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PhoneRevealRequestCWProxy {
  PhoneRevealRequest purpose(PhoneRevealRequestPurposeEnum purpose);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PhoneRevealRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PhoneRevealRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PhoneRevealRequest call({PhoneRevealRequestPurposeEnum purpose});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPhoneRevealRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPhoneRevealRequest.copyWith.fieldName(...)`
class _$PhoneRevealRequestCWProxyImpl implements _$PhoneRevealRequestCWProxy {
  const _$PhoneRevealRequestCWProxyImpl(this._value);

  final PhoneRevealRequest _value;

  @override
  PhoneRevealRequest purpose(PhoneRevealRequestPurposeEnum purpose) =>
      this(purpose: purpose);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PhoneRevealRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PhoneRevealRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PhoneRevealRequest call({Object? purpose = const $CopyWithPlaceholder()}) {
    return PhoneRevealRequest(
      purpose: purpose == const $CopyWithPlaceholder()
          ? _value.purpose
          // ignore: cast_nullable_to_non_nullable
          : purpose as PhoneRevealRequestPurposeEnum,
    );
  }
}

extension $PhoneRevealRequestCopyWith on PhoneRevealRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPhoneRevealRequest.copyWith(...)` or like so:`instanceOfPhoneRevealRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PhoneRevealRequestCWProxy get copyWith =>
      _$PhoneRevealRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PhoneRevealRequest _$PhoneRevealRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PhoneRevealRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['purpose']);
      final val = PhoneRevealRequest(
        purpose: $checkedConvert(
          'purpose',
          (v) => $enumDecode(
            _$PhoneRevealRequestPurposeEnumEnumMap,
            v,
            unknownValue: PhoneRevealRequestPurposeEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PhoneRevealRequestToJson(PhoneRevealRequest instance) =>
    <String, dynamic>{
      'purpose': _$PhoneRevealRequestPurposeEnumEnumMap[instance.purpose]!,
    };

const _$PhoneRevealRequestPurposeEnumEnumMap = {
  PhoneRevealRequestPurposeEnum.call: 'Call',
  PhoneRevealRequestPurposeEnum.sms: 'Sms',
  PhoneRevealRequestPurposeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
