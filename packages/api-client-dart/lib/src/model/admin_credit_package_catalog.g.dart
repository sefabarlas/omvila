// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_package_catalog.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditPackageCatalogCWProxy {
  AdminCreditPackageCatalog packages(List<AdminCreditPackage> packages);

  AdminCreditPackageCatalog version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditPackageCatalog(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditPackageCatalog(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditPackageCatalog call({
    List<AdminCreditPackage> packages,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditPackageCatalog.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditPackageCatalog.copyWith.fieldName(...)`
class _$AdminCreditPackageCatalogCWProxyImpl
    implements _$AdminCreditPackageCatalogCWProxy {
  const _$AdminCreditPackageCatalogCWProxyImpl(this._value);

  final AdminCreditPackageCatalog _value;

  @override
  AdminCreditPackageCatalog packages(List<AdminCreditPackage> packages) =>
      this(packages: packages);

  @override
  AdminCreditPackageCatalog version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditPackageCatalog(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditPackageCatalog(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditPackageCatalog call({
    Object? packages = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditPackageCatalog(
      packages: packages == const $CopyWithPlaceholder()
          ? _value.packages
          // ignore: cast_nullable_to_non_nullable
          : packages as List<AdminCreditPackage>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $AdminCreditPackageCatalogCopyWith on AdminCreditPackageCatalog {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditPackageCatalog.copyWith(...)` or like so:`instanceOfAdminCreditPackageCatalog.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditPackageCatalogCWProxy get copyWith =>
      _$AdminCreditPackageCatalogCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditPackageCatalog _$AdminCreditPackageCatalogFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminCreditPackageCatalog', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['packages', 'version']);
  final val = AdminCreditPackageCatalog(
    packages: $checkedConvert(
      'packages',
      (v) => (v as List<dynamic>)
          .map((e) => AdminCreditPackage.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminCreditPackageCatalogToJson(
  AdminCreditPackageCatalog instance,
) => <String, dynamic>{
  'packages': instance.packages.map((e) => e.toJson()).toList(),
  'version': instance.version,
};
