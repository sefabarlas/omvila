// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_package_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MePackageListCWProxy {
  MePackageList activeCount(int activeCount);

  MePackageList items(List<MePackage> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePackageList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePackageList(...).copyWith(id: 12, name: "My name")
  /// ````
  MePackageList call({int activeCount, List<MePackage> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMePackageList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMePackageList.copyWith.fieldName(...)`
class _$MePackageListCWProxyImpl implements _$MePackageListCWProxy {
  const _$MePackageListCWProxyImpl(this._value);

  final MePackageList _value;

  @override
  MePackageList activeCount(int activeCount) => this(activeCount: activeCount);

  @override
  MePackageList items(List<MePackage> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePackageList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePackageList(...).copyWith(id: 12, name: "My name")
  /// ````
  MePackageList call({
    Object? activeCount = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return MePackageList(
      activeCount: activeCount == const $CopyWithPlaceholder()
          ? _value.activeCount
          // ignore: cast_nullable_to_non_nullable
          : activeCount as int,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MePackage>,
    );
  }
}

extension $MePackageListCopyWith on MePackageList {
  /// Returns a callable class that can be used as follows: `instanceOfMePackageList.copyWith(...)` or like so:`instanceOfMePackageList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MePackageListCWProxy get copyWith => _$MePackageListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MePackageList _$MePackageListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MePackageList', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['activeCount', 'items']);
      final val = MePackageList(
        activeCount: $checkedConvert('activeCount', (v) => (v as num).toInt()),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => MePackage.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MePackageListToJson(MePackageList instance) =>
    <String, dynamic>{
      'activeCount': instance.activeCount,
      'items': instance.items.map((e) => e.toJson()).toList(),
    };
