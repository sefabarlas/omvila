// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_ref.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerRefCWProxy {
  CustomerRef brandCustomerId(String brandCustomerId);

  CustomerRef fullName(String fullName);

  CustomerRef phone(String? phone);

  CustomerRef phoneMasked(String? phoneMasked);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerRef(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerRef call({
    String brandCustomerId,
    String fullName,
    String? phone,
    String? phoneMasked,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerRef.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerRef.copyWith.fieldName(...)`
class _$CustomerRefCWProxyImpl implements _$CustomerRefCWProxy {
  const _$CustomerRefCWProxyImpl(this._value);

  final CustomerRef _value;

  @override
  CustomerRef brandCustomerId(String brandCustomerId) =>
      this(brandCustomerId: brandCustomerId);

  @override
  CustomerRef fullName(String fullName) => this(fullName: fullName);

  @override
  CustomerRef phone(String? phone) => this(phone: phone);

  @override
  CustomerRef phoneMasked(String? phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerRef(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerRef call({
    Object? brandCustomerId = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
  }) {
    return CustomerRef(
      brandCustomerId: brandCustomerId == const $CopyWithPlaceholder()
          ? _value.brandCustomerId
          // ignore: cast_nullable_to_non_nullable
          : brandCustomerId as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String?,
    );
  }
}

extension $CustomerRefCopyWith on CustomerRef {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerRef.copyWith(...)` or like so:`instanceOfCustomerRef.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerRefCWProxy get copyWith => _$CustomerRefCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerRef _$CustomerRefFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CustomerRef', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['brandCustomerId', 'fullName']);
      final val = CustomerRef(
        brandCustomerId: $checkedConvert('brandCustomerId', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String),
        phone: $checkedConvert('phone', (v) => v as String?),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$CustomerRefToJson(CustomerRef instance) =>
    <String, dynamic>{
      'brandCustomerId': instance.brandCustomerId,
      'fullName': instance.fullName,
      'phone': ?instance.phone,
      'phoneMasked': ?instance.phoneMasked,
    };
