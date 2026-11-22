// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'revealed_phone.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$RevealedPhoneCWProxy {
  RevealedPhone phone(String phone);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RevealedPhone(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RevealedPhone(...).copyWith(id: 12, name: "My name")
  /// ````
  RevealedPhone call({String phone});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfRevealedPhone.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfRevealedPhone.copyWith.fieldName(...)`
class _$RevealedPhoneCWProxyImpl implements _$RevealedPhoneCWProxy {
  const _$RevealedPhoneCWProxyImpl(this._value);

  final RevealedPhone _value;

  @override
  RevealedPhone phone(String phone) => this(phone: phone);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RevealedPhone(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RevealedPhone(...).copyWith(id: 12, name: "My name")
  /// ````
  RevealedPhone call({Object? phone = const $CopyWithPlaceholder()}) {
    return RevealedPhone(
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
    );
  }
}

extension $RevealedPhoneCopyWith on RevealedPhone {
  /// Returns a callable class that can be used as follows: `instanceOfRevealedPhone.copyWith(...)` or like so:`instanceOfRevealedPhone.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$RevealedPhoneCWProxy get copyWith => _$RevealedPhoneCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RevealedPhone _$RevealedPhoneFromJson(Map<String, dynamic> json) =>
    $checkedCreate('RevealedPhone', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['phone']);
      final val = RevealedPhone(
        phone: $checkedConvert('phone', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$RevealedPhoneToJson(RevealedPhone instance) =>
    <String, dynamic>{'phone': instance.phone};
