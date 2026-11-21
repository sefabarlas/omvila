// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageListCWProxy {
  PackageList items(List<PackageDefinition> items);

  PackageList overview(PackageOverview overview);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageList(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageList call({List<PackageDefinition> items, PackageOverview overview});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageList.copyWith.fieldName(...)`
class _$PackageListCWProxyImpl implements _$PackageListCWProxy {
  const _$PackageListCWProxyImpl(this._value);

  final PackageList _value;

  @override
  PackageList items(List<PackageDefinition> items) => this(items: items);

  @override
  PackageList overview(PackageOverview overview) => this(overview: overview);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageList(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageList call({
    Object? items = const $CopyWithPlaceholder(),
    Object? overview = const $CopyWithPlaceholder(),
  }) {
    return PackageList(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PackageDefinition>,
      overview: overview == const $CopyWithPlaceholder()
          ? _value.overview
          // ignore: cast_nullable_to_non_nullable
          : overview as PackageOverview,
    );
  }
}

extension $PackageListCopyWith on PackageList {
  /// Returns a callable class that can be used as follows: `instanceOfPackageList.copyWith(...)` or like so:`instanceOfPackageList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageListCWProxy get copyWith => _$PackageListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageList _$PackageListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PackageList', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'overview']);
      final val = PackageList(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => PackageDefinition.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        overview: $checkedConvert(
          'overview',
          (v) => PackageOverview.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PackageListToJson(PackageList instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'overview': instance.overview.toJson(),
    };
