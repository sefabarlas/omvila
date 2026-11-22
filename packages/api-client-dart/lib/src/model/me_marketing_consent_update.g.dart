// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_marketing_consent_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeMarketingConsentUpdateCWProxy {
  MeMarketingConsentUpdate brandId(String brandId);

  MeMarketingConsentUpdate granted(bool granted);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeMarketingConsentUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeMarketingConsentUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  MeMarketingConsentUpdate call({String brandId, bool granted});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeMarketingConsentUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeMarketingConsentUpdate.copyWith.fieldName(...)`
class _$MeMarketingConsentUpdateCWProxyImpl
    implements _$MeMarketingConsentUpdateCWProxy {
  const _$MeMarketingConsentUpdateCWProxyImpl(this._value);

  final MeMarketingConsentUpdate _value;

  @override
  MeMarketingConsentUpdate brandId(String brandId) => this(brandId: brandId);

  @override
  MeMarketingConsentUpdate granted(bool granted) => this(granted: granted);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeMarketingConsentUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeMarketingConsentUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  MeMarketingConsentUpdate call({
    Object? brandId = const $CopyWithPlaceholder(),
    Object? granted = const $CopyWithPlaceholder(),
  }) {
    return MeMarketingConsentUpdate(
      brandId: brandId == const $CopyWithPlaceholder()
          ? _value.brandId
          // ignore: cast_nullable_to_non_nullable
          : brandId as String,
      granted: granted == const $CopyWithPlaceholder()
          ? _value.granted
          // ignore: cast_nullable_to_non_nullable
          : granted as bool,
    );
  }
}

extension $MeMarketingConsentUpdateCopyWith on MeMarketingConsentUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfMeMarketingConsentUpdate.copyWith(...)` or like so:`instanceOfMeMarketingConsentUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeMarketingConsentUpdateCWProxy get copyWith =>
      _$MeMarketingConsentUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeMarketingConsentUpdate _$MeMarketingConsentUpdateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeMarketingConsentUpdate', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['brandId', 'granted']);
  final val = MeMarketingConsentUpdate(
    brandId: $checkedConvert('brandId', (v) => v as String),
    granted: $checkedConvert('granted', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$MeMarketingConsentUpdateToJson(
  MeMarketingConsentUpdate instance,
) => <String, dynamic>{
  'brandId': instance.brandId,
  'granted': instance.granted,
};
