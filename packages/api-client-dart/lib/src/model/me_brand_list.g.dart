// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_brand_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeBrandListCWProxy {
  MeBrandList items(List<MeBrand> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrandList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrandList(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrandList call({List<MeBrand> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeBrandList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeBrandList.copyWith.fieldName(...)`
class _$MeBrandListCWProxyImpl implements _$MeBrandListCWProxy {
  const _$MeBrandListCWProxyImpl(this._value);

  final MeBrandList _value;

  @override
  MeBrandList items(List<MeBrand> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrandList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrandList(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrandList call({Object? items = const $CopyWithPlaceholder()}) {
    return MeBrandList(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MeBrand>,
    );
  }
}

extension $MeBrandListCopyWith on MeBrandList {
  /// Returns a callable class that can be used as follows: `instanceOfMeBrandList.copyWith(...)` or like so:`instanceOfMeBrandList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeBrandListCWProxy get copyWith => _$MeBrandListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeBrandList _$MeBrandListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeBrandList', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = MeBrandList(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => MeBrand.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeBrandListToJson(MeBrandList instance) =>
    <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
