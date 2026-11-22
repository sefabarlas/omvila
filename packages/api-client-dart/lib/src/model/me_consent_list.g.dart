// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_consent_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeConsentListCWProxy {
  MeConsentList items(List<MeConsent> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeConsentList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeConsentList(...).copyWith(id: 12, name: "My name")
  /// ````
  MeConsentList call({List<MeConsent> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeConsentList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeConsentList.copyWith.fieldName(...)`
class _$MeConsentListCWProxyImpl implements _$MeConsentListCWProxy {
  const _$MeConsentListCWProxyImpl(this._value);

  final MeConsentList _value;

  @override
  MeConsentList items(List<MeConsent> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeConsentList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeConsentList(...).copyWith(id: 12, name: "My name")
  /// ````
  MeConsentList call({Object? items = const $CopyWithPlaceholder()}) {
    return MeConsentList(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MeConsent>,
    );
  }
}

extension $MeConsentListCopyWith on MeConsentList {
  /// Returns a callable class that can be used as follows: `instanceOfMeConsentList.copyWith(...)` or like so:`instanceOfMeConsentList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeConsentListCWProxy get copyWith => _$MeConsentListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeConsentList _$MeConsentListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeConsentList', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = MeConsentList(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => MeConsent.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeConsentListToJson(MeConsentList instance) =>
    <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
