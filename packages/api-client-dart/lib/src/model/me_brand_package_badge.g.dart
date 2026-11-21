// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_brand_package_badge.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeBrandPackageBadgeCWProxy {
  MeBrandPackageBadge customerPackageId(String customerPackageId);

  MeBrandPackageBadge packageName(String packageName);

  MeBrandPackageBadge remainingSessions(int remainingSessions);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrandPackageBadge(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrandPackageBadge(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrandPackageBadge call({
    String customerPackageId,
    String packageName,
    int remainingSessions,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeBrandPackageBadge.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeBrandPackageBadge.copyWith.fieldName(...)`
class _$MeBrandPackageBadgeCWProxyImpl implements _$MeBrandPackageBadgeCWProxy {
  const _$MeBrandPackageBadgeCWProxyImpl(this._value);

  final MeBrandPackageBadge _value;

  @override
  MeBrandPackageBadge customerPackageId(String customerPackageId) =>
      this(customerPackageId: customerPackageId);

  @override
  MeBrandPackageBadge packageName(String packageName) =>
      this(packageName: packageName);

  @override
  MeBrandPackageBadge remainingSessions(int remainingSessions) =>
      this(remainingSessions: remainingSessions);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrandPackageBadge(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrandPackageBadge(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrandPackageBadge call({
    Object? customerPackageId = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
  }) {
    return MeBrandPackageBadge(
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

extension $MeBrandPackageBadgeCopyWith on MeBrandPackageBadge {
  /// Returns a callable class that can be used as follows: `instanceOfMeBrandPackageBadge.copyWith(...)` or like so:`instanceOfMeBrandPackageBadge.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeBrandPackageBadgeCWProxy get copyWith =>
      _$MeBrandPackageBadgeCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeBrandPackageBadge _$MeBrandPackageBadgeFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeBrandPackageBadge', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'customerPackageId',
          'packageName',
          'remainingSessions',
        ],
      );
      final val = MeBrandPackageBadge(
        customerPackageId: $checkedConvert(
          'customerPackageId',
          (v) => v as String,
        ),
        packageName: $checkedConvert('packageName', (v) => v as String),
        remainingSessions: $checkedConvert(
          'remainingSessions',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeBrandPackageBadgeToJson(
  MeBrandPackageBadge instance,
) => <String, dynamic>{
  'customerPackageId': instance.customerPackageId,
  'packageName': instance.packageName,
  'remainingSessions': instance.remainingSessions,
};
