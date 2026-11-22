// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_write.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageWriteCWProxy {
  PackageWrite name(String name);

  PackageWrite price(Money price);

  PackageWrite items(List<PackageItemWrite> items);

  PackageWrite validityDays(int validityDays);

  PackageWrite saleCommissionPercent(num? saleCommissionPercent);

  PackageWrite isAvailableAllBranches(bool isAvailableAllBranches);

  PackageWrite isActive(bool isActive);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageWrite call({
    String name,
    Money price,
    List<PackageItemWrite> items,
    int validityDays,
    num? saleCommissionPercent,
    bool isAvailableAllBranches,
    bool isActive,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageWrite.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageWrite.copyWith.fieldName(...)`
class _$PackageWriteCWProxyImpl implements _$PackageWriteCWProxy {
  const _$PackageWriteCWProxyImpl(this._value);

  final PackageWrite _value;

  @override
  PackageWrite name(String name) => this(name: name);

  @override
  PackageWrite price(Money price) => this(price: price);

  @override
  PackageWrite items(List<PackageItemWrite> items) => this(items: items);

  @override
  PackageWrite validityDays(int validityDays) =>
      this(validityDays: validityDays);

  @override
  PackageWrite saleCommissionPercent(num? saleCommissionPercent) =>
      this(saleCommissionPercent: saleCommissionPercent);

  @override
  PackageWrite isAvailableAllBranches(bool isAvailableAllBranches) =>
      this(isAvailableAllBranches: isAvailableAllBranches);

  @override
  PackageWrite isActive(bool isActive) => this(isActive: isActive);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageWrite call({
    Object? name = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? validityDays = const $CopyWithPlaceholder(),
    Object? saleCommissionPercent = const $CopyWithPlaceholder(),
    Object? isAvailableAllBranches = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
  }) {
    return PackageWrite(
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
          : items as List<PackageItemWrite>,
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
    );
  }
}

extension $PackageWriteCopyWith on PackageWrite {
  /// Returns a callable class that can be used as follows: `instanceOfPackageWrite.copyWith(...)` or like so:`instanceOfPackageWrite.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageWriteCWProxy get copyWith => _$PackageWriteCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageWrite _$PackageWriteFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PackageWrite', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'name',
          'price',
          'items',
          'validityDays',
          'isAvailableAllBranches',
          'isActive',
        ],
      );
      final val = PackageWrite(
        name: $checkedConvert('name', (v) => v as String),
        price: $checkedConvert(
          'price',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => PackageItemWrite.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        validityDays: $checkedConvert(
          'validityDays',
          (v) => (v as num).toInt(),
        ),
        saleCommissionPercent: $checkedConvert(
          'saleCommissionPercent',
          (v) => v as num?,
        ),
        isAvailableAllBranches: $checkedConvert(
          'isAvailableAllBranches',
          (v) => v as bool,
        ),
        isActive: $checkedConvert('isActive', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$PackageWriteToJson(PackageWrite instance) =>
    <String, dynamic>{
      'name': instance.name,
      'price': instance.price.toJson(),
      'items': instance.items.map((e) => e.toJson()).toList(),
      'validityDays': instance.validityDays,
      'saleCommissionPercent': instance.saleCommissionPercent,
      'isAvailableAllBranches': instance.isAvailableAllBranches,
      'isActive': instance.isActive,
    };
