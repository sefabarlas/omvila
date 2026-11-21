// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_consent.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeConsentCWProxy {
  MeConsent brand(MeBrandRef brand);

  MeConsent marketingGranted(bool marketingGranted);

  MeConsent updatedUtc(DateTime? updatedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeConsent(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeConsent(...).copyWith(id: 12, name: "My name")
  /// ````
  MeConsent call({
    MeBrandRef brand,
    bool marketingGranted,
    DateTime? updatedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeConsent.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeConsent.copyWith.fieldName(...)`
class _$MeConsentCWProxyImpl implements _$MeConsentCWProxy {
  const _$MeConsentCWProxyImpl(this._value);

  final MeConsent _value;

  @override
  MeConsent brand(MeBrandRef brand) => this(brand: brand);

  @override
  MeConsent marketingGranted(bool marketingGranted) =>
      this(marketingGranted: marketingGranted);

  @override
  MeConsent updatedUtc(DateTime? updatedUtc) => this(updatedUtc: updatedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeConsent(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeConsent(...).copyWith(id: 12, name: "My name")
  /// ````
  MeConsent call({
    Object? brand = const $CopyWithPlaceholder(),
    Object? marketingGranted = const $CopyWithPlaceholder(),
    Object? updatedUtc = const $CopyWithPlaceholder(),
  }) {
    return MeConsent(
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as MeBrandRef,
      marketingGranted: marketingGranted == const $CopyWithPlaceholder()
          ? _value.marketingGranted
          // ignore: cast_nullable_to_non_nullable
          : marketingGranted as bool,
      updatedUtc: updatedUtc == const $CopyWithPlaceholder()
          ? _value.updatedUtc
          // ignore: cast_nullable_to_non_nullable
          : updatedUtc as DateTime?,
    );
  }
}

extension $MeConsentCopyWith on MeConsent {
  /// Returns a callable class that can be used as follows: `instanceOfMeConsent.copyWith(...)` or like so:`instanceOfMeConsent.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeConsentCWProxy get copyWith => _$MeConsentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeConsent _$MeConsentFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeConsent', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['brand', 'marketingGranted', 'updatedUtc'],
      );
      final val = MeConsent(
        brand: $checkedConvert(
          'brand',
          (v) => MeBrandRef.fromJson(v as Map<String, dynamic>),
        ),
        marketingGranted: $checkedConvert('marketingGranted', (v) => v as bool),
        updatedUtc: $checkedConvert(
          'updatedUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeConsentToJson(MeConsent instance) => <String, dynamic>{
  'brand': instance.brand.toJson(),
  'marketingGranted': instance.marketingGranted,
  'updatedUtc': instance.updatedUtc?.toIso8601String(),
};
