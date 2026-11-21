// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_definition.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageDefinitionCWProxy {
  PackageDefinition id(String id);

  PackageDefinition name(String name);

  PackageDefinition price(Money price);

  PackageDefinition items(List<PackageDefinitionItemsInner> items);

  PackageDefinition totalSessions(int totalSessions);

  PackageDefinition validityDays(int validityDays);

  PackageDefinition saleCommissionPercent(num? saleCommissionPercent);

  PackageDefinition isAvailableAllBranches(bool isAvailableAllBranches);

  PackageDefinition isActive(bool isActive);

  PackageDefinition soldCount(int soldCount);

  PackageDefinition activeCount(int activeCount);

  PackageDefinition version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageDefinition(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageDefinition(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageDefinition call({
    String id,
    String name,
    Money price,
    List<PackageDefinitionItemsInner> items,
    int totalSessions,
    int validityDays,
    num? saleCommissionPercent,
    bool isAvailableAllBranches,
    bool isActive,
    int soldCount,
    int activeCount,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageDefinition.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageDefinition.copyWith.fieldName(...)`
class _$PackageDefinitionCWProxyImpl implements _$PackageDefinitionCWProxy {
  const _$PackageDefinitionCWProxyImpl(this._value);

  final PackageDefinition _value;

  @override
  PackageDefinition id(String id) => this(id: id);

  @override
  PackageDefinition name(String name) => this(name: name);

  @override
  PackageDefinition price(Money price) => this(price: price);

  @override
  PackageDefinition items(List<PackageDefinitionItemsInner> items) =>
      this(items: items);

  @override
  PackageDefinition totalSessions(int totalSessions) =>
      this(totalSessions: totalSessions);

  @override
  PackageDefinition validityDays(int validityDays) =>
      this(validityDays: validityDays);

  @override
  PackageDefinition saleCommissionPercent(num? saleCommissionPercent) =>
      this(saleCommissionPercent: saleCommissionPercent);

  @override
  PackageDefinition isAvailableAllBranches(bool isAvailableAllBranches) =>
      this(isAvailableAllBranches: isAvailableAllBranches);

  @override
  PackageDefinition isActive(bool isActive) => this(isActive: isActive);

  @override
  PackageDefinition soldCount(int soldCount) => this(soldCount: soldCount);

  @override
  PackageDefinition activeCount(int activeCount) =>
      this(activeCount: activeCount);

  @override
  PackageDefinition version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageDefinition(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageDefinition(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageDefinition call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? totalSessions = const $CopyWithPlaceholder(),
    Object? validityDays = const $CopyWithPlaceholder(),
    Object? saleCommissionPercent = const $CopyWithPlaceholder(),
    Object? isAvailableAllBranches = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? soldCount = const $CopyWithPlaceholder(),
    Object? activeCount = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return PackageDefinition(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PackageDefinitionItemsInner>,
      totalSessions: totalSessions == const $CopyWithPlaceholder()
          ? _value.totalSessions
          // ignore: cast_nullable_to_non_nullable
          : totalSessions as int,
      validityDays: validityDays == const $CopyWithPlaceholder()
          ? _value.validityDays
          // ignore: cast_nullable_to_non_nullable
          : validityDays as int,
      saleCommissionPercent:
          saleCommissionPercent == const $CopyWithPlaceholder()
          ? _value.saleCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : saleCommissionPercent as num?,
      isAvailableAllBranches:
          isAvailableAllBranches == const $CopyWithPlaceholder()
          ? _value.isAvailableAllBranches
          // ignore: cast_nullable_to_non_nullable
          : isAvailableAllBranches as bool,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
      soldCount: soldCount == const $CopyWithPlaceholder()
          ? _value.soldCount
          // ignore: cast_nullable_to_non_nullable
          : soldCount as int,
      activeCount: activeCount == const $CopyWithPlaceholder()
          ? _value.activeCount
          // ignore: cast_nullable_to_non_nullable
          : activeCount as int,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $PackageDefinitionCopyWith on PackageDefinition {
  /// Returns a callable class that can be used as follows: `instanceOfPackageDefinition.copyWith(...)` or like so:`instanceOfPackageDefinition.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageDefinitionCWProxy get copyWith =>
      _$PackageDefinitionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageDefinition _$PackageDefinitionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PackageDefinition', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'name',
      'price',
      'items',
      'totalSessions',
      'validityDays',
      'isAvailableAllBranches',
      'isActive',
      'soldCount',
      'activeCount',
      'version',
    ],
  );
  final val = PackageDefinition(
    id: $checkedConvert('id', (v) => v as String),
    name: $checkedConvert('name', (v) => v as String),
    price: $checkedConvert(
      'price',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) =>
                PackageDefinitionItemsInner.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    totalSessions: $checkedConvert('totalSessions', (v) => (v as num).toInt()),
    validityDays: $checkedConvert('validityDays', (v) => (v as num).toInt()),
    saleCommissionPercent: $checkedConvert(
      'saleCommissionPercent',
      (v) => v as num?,
    ),
    isAvailableAllBranches: $checkedConvert(
      'isAvailableAllBranches',
      (v) => v as bool,
    ),
    isActive: $checkedConvert('isActive', (v) => v as bool),
    soldCount: $checkedConvert('soldCount', (v) => (v as num).toInt()),
    activeCount: $checkedConvert('activeCount', (v) => (v as num).toInt()),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$PackageDefinitionToJson(PackageDefinition instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'price': instance.price.toJson(),
      'items': instance.items.map((e) => e.toJson()).toList(),
      'totalSessions': instance.totalSessions,
      'validityDays': instance.validityDays,
      'saleCommissionPercent': ?instance.saleCommissionPercent,
      'isAvailableAllBranches': instance.isAvailableAllBranches,
      'isActive': instance.isActive,
      'soldCount': instance.soldCount,
      'activeCount': instance.activeCount,
      'version': instance.version,
    };
