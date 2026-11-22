// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_package_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CreditPackageListCWProxy {
  CreditPackageList items(List<CreditPackage> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPackageList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPackageList(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPackageList call({List<CreditPackage> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCreditPackageList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCreditPackageList.copyWith.fieldName(...)`
class _$CreditPackageListCWProxyImpl implements _$CreditPackageListCWProxy {
  const _$CreditPackageListCWProxyImpl(this._value);

  final CreditPackageList _value;

  @override
  CreditPackageList items(List<CreditPackage> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPackageList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPackageList(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPackageList call({Object? items = const $CopyWithPlaceholder()}) {
    return CreditPackageList(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<CreditPackage>,
    );
  }
}

extension $CreditPackageListCopyWith on CreditPackageList {
  /// Returns a callable class that can be used as follows: `instanceOfCreditPackageList.copyWith(...)` or like so:`instanceOfCreditPackageList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CreditPackageListCWProxy get copyWith =>
      _$CreditPackageListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreditPackageList _$CreditPackageListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CreditPackageList', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = CreditPackageList(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => CreditPackage.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CreditPackageListToJson(CreditPackageList instance) =>
    <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
