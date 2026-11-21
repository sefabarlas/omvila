// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_deletion_preview_active_packages_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerDeletionPreviewActivePackagesInnerCWProxy {
  CustomerDeletionPreviewActivePackagesInner customerPackageId(
    String customerPackageId,
  );

  CustomerDeletionPreviewActivePackagesInner packageName(String packageName);

  CustomerDeletionPreviewActivePackagesInner remainingSessions(
    int remainingSessions,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerDeletionPreviewActivePackagesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerDeletionPreviewActivePackagesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerDeletionPreviewActivePackagesInner call({
    String customerPackageId,
    String packageName,
    int remainingSessions,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerDeletionPreviewActivePackagesInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerDeletionPreviewActivePackagesInner.copyWith.fieldName(...)`
class _$CustomerDeletionPreviewActivePackagesInnerCWProxyImpl
    implements _$CustomerDeletionPreviewActivePackagesInnerCWProxy {
  const _$CustomerDeletionPreviewActivePackagesInnerCWProxyImpl(this._value);

  final CustomerDeletionPreviewActivePackagesInner _value;

  @override
  CustomerDeletionPreviewActivePackagesInner customerPackageId(
    String customerPackageId,
  ) => this(customerPackageId: customerPackageId);

  @override
  CustomerDeletionPreviewActivePackagesInner packageName(String packageName) =>
      this(packageName: packageName);

  @override
  CustomerDeletionPreviewActivePackagesInner remainingSessions(
    int remainingSessions,
  ) => this(remainingSessions: remainingSessions);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerDeletionPreviewActivePackagesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerDeletionPreviewActivePackagesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerDeletionPreviewActivePackagesInner call({
    Object? customerPackageId = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
  }) {
    return CustomerDeletionPreviewActivePackagesInner(
      customerPackageId: customerPackageId == const $CopyWithPlaceholder()
          ? _value.customerPackageId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageId as String,
      packageName: packageName == const $CopyWithPlaceholder()
          ? _value.packageName
          // ignore: cast_nullable_to_non_nullable
          : packageName as String,
      remainingSessions: remainingSessions == const $CopyWithPlaceholder()
          ? _value.remainingSessions
          // ignore: cast_nullable_to_non_nullable
          : remainingSessions as int,
    );
  }
}

extension $CustomerDeletionPreviewActivePackagesInnerCopyWith
    on CustomerDeletionPreviewActivePackagesInner {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerDeletionPreviewActivePackagesInner.copyWith(...)` or like so:`instanceOfCustomerDeletionPreviewActivePackagesInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerDeletionPreviewActivePackagesInnerCWProxy get copyWith =>
      _$CustomerDeletionPreviewActivePackagesInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerDeletionPreviewActivePackagesInner
_$CustomerDeletionPreviewActivePackagesInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerDeletionPreviewActivePackagesInner', json, (
  $checkedConvert,
) {
  $checkKeys(
    json,
    requiredKeys: const [
      'customerPackageId',
      'packageName',
      'remainingSessions',
    ],
  );
  final val = CustomerDeletionPreviewActivePackagesInner(
    customerPackageId: $checkedConvert('customerPackageId', (v) => v as String),
    packageName: $checkedConvert('packageName', (v) => v as String),
    remainingSessions: $checkedConvert(
      'remainingSessions',
      (v) => (v as num).toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$CustomerDeletionPreviewActivePackagesInnerToJson(
  CustomerDeletionPreviewActivePackagesInner instance,
) => <String, dynamic>{
  'customerPackageId': instance.customerPackageId,
  'packageName': instance.packageName,
  'remainingSessions': instance.remainingSessions,
};
