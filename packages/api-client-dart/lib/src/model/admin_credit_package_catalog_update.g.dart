// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_package_catalog_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditPackageCatalogUpdateCWProxy {
  AdminCreditPackageCatalogUpdate packages(List<AdminCreditPackage> packages);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditPackageCatalogUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditPackageCatalogUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditPackageCatalogUpdate call({List<AdminCreditPackage> packages});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditPackageCatalogUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditPackageCatalogUpdate.copyWith.fieldName(...)`
class _$AdminCreditPackageCatalogUpdateCWProxyImpl
    implements _$AdminCreditPackageCatalogUpdateCWProxy {
  const _$AdminCreditPackageCatalogUpdateCWProxyImpl(this._value);

  final AdminCreditPackageCatalogUpdate _value;

  @override
  AdminCreditPackageCatalogUpdate packages(List<AdminCreditPackage> packages) =>
      this(packages: packages);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditPackageCatalogUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditPackageCatalogUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditPackageCatalogUpdate call({
    Object? packages = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditPackageCatalogUpdate(
      packages: packages == const $CopyWithPlaceholder()
          ? _value.packages
          // ignore: cast_nullable_to_non_nullable
          : packages as List<AdminCreditPackage>,
    );
  }
}

extension $AdminCreditPackageCatalogUpdateCopyWith
    on AdminCreditPackageCatalogUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditPackageCatalogUpdate.copyWith(...)` or like so:`instanceOfAdminCreditPackageCatalogUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditPackageCatalogUpdateCWProxy get copyWith =>
      _$AdminCreditPackageCatalogUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditPackageCatalogUpdate _$AdminCreditPackageCatalogUpdateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminCreditPackageCatalogUpdate', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['packages']);
  final val = AdminCreditPackageCatalogUpdate(
    packages: $checkedConvert(
      'packages',
      (v) => (v as List<dynamic>)
          .map((e) => AdminCreditPackage.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminCreditPackageCatalogUpdateToJson(
  AdminCreditPackageCatalogUpdate instance,
) => <String, dynamic>{
  'packages': instance.packages.map((e) => e.toJson()).toList(),
};
