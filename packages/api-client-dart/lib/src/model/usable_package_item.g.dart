// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'usable_package_item.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$UsablePackageItemCWProxy {
  UsablePackageItem customerPackageItemId(String customerPackageItemId);

  UsablePackageItem serviceId(String serviceId);

  UsablePackageItem packageName(String packageName);

  UsablePackageItem remainingSessions(int remainingSessions);

  UsablePackageItem totalSessions(int totalSessions);

  UsablePackageItem expiresOn(DateTime? expiresOn);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `UsablePackageItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// UsablePackageItem(...).copyWith(id: 12, name: "My name")
  /// ````
  UsablePackageItem call({
    String customerPackageItemId,
    String serviceId,
    String packageName,
    int remainingSessions,
    int totalSessions,
    DateTime? expiresOn,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfUsablePackageItem.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfUsablePackageItem.copyWith.fieldName(...)`
class _$UsablePackageItemCWProxyImpl implements _$UsablePackageItemCWProxy {
  const _$UsablePackageItemCWProxyImpl(this._value);

  final UsablePackageItem _value;

  @override
  UsablePackageItem customerPackageItemId(String customerPackageItemId) =>
      this(customerPackageItemId: customerPackageItemId);

  @override
  UsablePackageItem serviceId(String serviceId) => this(serviceId: serviceId);

  @override
  UsablePackageItem packageName(String packageName) =>
      this(packageName: packageName);

  @override
  UsablePackageItem remainingSessions(int remainingSessions) =>
      this(remainingSessions: remainingSessions);

  @override
  UsablePackageItem totalSessions(int totalSessions) =>
      this(totalSessions: totalSessions);

  @override
  UsablePackageItem expiresOn(DateTime? expiresOn) =>
      this(expiresOn: expiresOn);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `UsablePackageItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// UsablePackageItem(...).copyWith(id: 12, name: "My name")
  /// ````
  UsablePackageItem call({
    Object? customerPackageItemId = const $CopyWithPlaceholder(),
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
    Object? totalSessions = const $CopyWithPlaceholder(),
    Object? expiresOn = const $CopyWithPlaceholder(),
  }) {
    return UsablePackageItem(
      customerPackageItemId:
          customerPackageItemId == const $CopyWithPlaceholder()
          ? _value.customerPackageItemId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageItemId as String,
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      packageName: packageName == const $CopyWithPlaceholder()
          ? _value.packageName
          // ignore: cast_nullable_to_non_nullable
          : packageName as String,
      remainingSessions: remainingSessions == const $CopyWithPlaceholder()
          ? _value.remainingSessions
          // ignore: cast_nullable_to_non_nullable
          : remainingSessions as int,
      totalSessions: totalSessions == const $CopyWithPlaceholder()
          ? _value.totalSessions
          // ignore: cast_nullable_to_non_nullable
          : totalSessions as int,
      expiresOn: expiresOn == const $CopyWithPlaceholder()
          ? _value.expiresOn
          // ignore: cast_nullable_to_non_nullable
          : expiresOn as DateTime?,
    );
  }
}

extension $UsablePackageItemCopyWith on UsablePackageItem {
  /// Returns a callable class that can be used as follows: `instanceOfUsablePackageItem.copyWith(...)` or like so:`instanceOfUsablePackageItem.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$UsablePackageItemCWProxy get copyWith =>
      _$UsablePackageItemCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UsablePackageItem _$UsablePackageItemFromJson(Map<String, dynamic> json) =>
    $checkedCreate('UsablePackageItem', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'customerPackageItemId',
          'serviceId',
          'packageName',
          'remainingSessions',
          'totalSessions',
        ],
      );
      final val = UsablePackageItem(
        customerPackageItemId: $checkedConvert(
          'customerPackageItemId',
          (v) => v as String,
        ),
        serviceId: $checkedConvert('serviceId', (v) => v as String),
        packageName: $checkedConvert('packageName', (v) => v as String),
        remainingSessions: $checkedConvert(
          'remainingSessions',
          (v) => (v as num).toInt(),
        ),
        totalSessions: $checkedConvert(
          'totalSessions',
          (v) => (v as num).toInt(),
        ),
        expiresOn: $checkedConvert(
          'expiresOn',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$UsablePackageItemToJson(UsablePackageItem instance) =>
    <String, dynamic>{
      'customerPackageItemId': instance.customerPackageItemId,
      'serviceId': instance.serviceId,
      'packageName': instance.packageName,
      'remainingSessions': instance.remainingSessions,
      'totalSessions': instance.totalSessions,
      'expiresOn': ?instance.expiresOn?.toIso8601String(),
    };
