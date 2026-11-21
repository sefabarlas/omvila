// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_package_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerPackagePageCWProxy {
  CustomerPackagePage items(List<CustomerPackage> items);

  CustomerPackagePage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPackagePage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPackagePage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPackagePage call({List<CustomerPackage> items, String? nextCursor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerPackagePage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerPackagePage.copyWith.fieldName(...)`
class _$CustomerPackagePageCWProxyImpl implements _$CustomerPackagePageCWProxy {
  const _$CustomerPackagePageCWProxyImpl(this._value);

  final CustomerPackagePage _value;

  @override
  CustomerPackagePage items(List<CustomerPackage> items) => this(items: items);

  @override
  CustomerPackagePage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPackagePage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPackagePage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPackagePage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return CustomerPackagePage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<CustomerPackage>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $CustomerPackagePageCopyWith on CustomerPackagePage {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerPackagePage.copyWith(...)` or like so:`instanceOfCustomerPackagePage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerPackagePageCWProxy get copyWith =>
      _$CustomerPackagePageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerPackagePage _$CustomerPackagePageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CustomerPackagePage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = CustomerPackagePage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => CustomerPackage.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$CustomerPackagePageToJson(
  CustomerPackagePage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
