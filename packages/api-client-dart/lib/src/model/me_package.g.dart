// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_package.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MePackageCWProxy {
  MePackage id(String id);

  MePackage brand(MeBrandRef brand);

  MePackage branchName(String? branchName);

  MePackage isAvailableAllBranches(bool isAvailableAllBranches);

  MePackage packageName(String packageName);

  MePackage status(CustomerPackageStatus status);

  MePackage items(List<CustomerPackageItemView> items);

  MePackage totalSessions(int totalSessions);

  MePackage remainingSessions(int remainingSessions);

  MePackage expiresOn(DateTime expiresOn);

  MePackage isUsable(bool isUsable);

  MePackage isRunningLow(bool isRunningLow);

  MePackage finishedOn(DateTime? finishedOn);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePackage(...).copyWith(id: 12, name: "My name")
  /// ````
  MePackage call({
    String id,
    MeBrandRef brand,
    String? branchName,
    bool isAvailableAllBranches,
    String packageName,
    CustomerPackageStatus status,
    List<CustomerPackageItemView> items,
    int totalSessions,
    int remainingSessions,
    DateTime expiresOn,
    bool isUsable,
    bool isRunningLow,
    DateTime? finishedOn,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMePackage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMePackage.copyWith.fieldName(...)`
class _$MePackageCWProxyImpl implements _$MePackageCWProxy {
  const _$MePackageCWProxyImpl(this._value);

  final MePackage _value;

  @override
  MePackage id(String id) => this(id: id);

  @override
  MePackage brand(MeBrandRef brand) => this(brand: brand);

  @override
  MePackage branchName(String? branchName) => this(branchName: branchName);

  @override
  MePackage isAvailableAllBranches(bool isAvailableAllBranches) =>
      this(isAvailableAllBranches: isAvailableAllBranches);

  @override
  MePackage packageName(String packageName) => this(packageName: packageName);

  @override
  MePackage status(CustomerPackageStatus status) => this(status: status);

  @override
  MePackage items(List<CustomerPackageItemView> items) => this(items: items);

  @override
  MePackage totalSessions(int totalSessions) =>
      this(totalSessions: totalSessions);

  @override
  MePackage remainingSessions(int remainingSessions) =>
      this(remainingSessions: remainingSessions);

  @override
  MePackage expiresOn(DateTime expiresOn) => this(expiresOn: expiresOn);

  @override
  MePackage isUsable(bool isUsable) => this(isUsable: isUsable);

  @override
  MePackage isRunningLow(bool isRunningLow) => this(isRunningLow: isRunningLow);

  @override
  MePackage finishedOn(DateTime? finishedOn) => this(finishedOn: finishedOn);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePackage(...).copyWith(id: 12, name: "My name")
  /// ````
  MePackage call({
    Object? id = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? branchName = const $CopyWithPlaceholder(),
    Object? isAvailableAllBranches = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? totalSessions = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
    Object? expiresOn = const $CopyWithPlaceholder(),
    Object? isUsable = const $CopyWithPlaceholder(),
    Object? isRunningLow = const $CopyWithPlaceholder(),
    Object? finishedOn = const $CopyWithPlaceholder(),
  }) {
    return MePackage(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as MeBrandRef,
      branchName: branchName == const $CopyWithPlaceholder()
          ? _value.branchName
          // ignore: cast_nullable_to_non_nullable
          : branchName as String?,
      isAvailableAllBranches:
          isAvailableAllBranches == const $CopyWithPlaceholder()
          ? _value.isAvailableAllBranches
          // ignore: cast_nullable_to_non_nullable
          : isAvailableAllBranches as bool,
      packageName: packageName == const $CopyWithPlaceholder()
          ? _value.packageName
          // ignore: cast_nullable_to_non_nullable
          : packageName as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as CustomerPackageStatus,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<CustomerPackageItemView>,
      totalSessions: totalSessions == const $CopyWithPlaceholder()
          ? _value.totalSessions
          // ignore: cast_nullable_to_non_nullable
          : totalSessions as int,
      remainingSessions: remainingSessions == const $CopyWithPlaceholder()
          ? _value.remainingSessions
          // ignore: cast_nullable_to_non_nullable
          : remainingSessions as int,
      expiresOn: expiresOn == const $CopyWithPlaceholder()
          ? _value.expiresOn
          // ignore: cast_nullable_to_non_nullable
          : expiresOn as DateTime,
      isUsable: isUsable == const $CopyWithPlaceholder()
          ? _value.isUsable
          // ignore: cast_nullable_to_non_nullable
          : isUsable as bool,
      isRunningLow: isRunningLow == const $CopyWithPlaceholder()
          ? _value.isRunningLow
          // ignore: cast_nullable_to_non_nullable
          : isRunningLow as bool,
      finishedOn: finishedOn == const $CopyWithPlaceholder()
          ? _value.finishedOn
          // ignore: cast_nullable_to_non_nullable
          : finishedOn as DateTime?,
    );
  }
}

extension $MePackageCopyWith on MePackage {
  /// Returns a callable class that can be used as follows: `instanceOfMePackage.copyWith(...)` or like so:`instanceOfMePackage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MePackageCWProxy get copyWith => _$MePackageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MePackage _$MePackageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MePackage', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'brand',
      'branchName',
      'isAvailableAllBranches',
      'packageName',
      'status',
      'items',
      'totalSessions',
      'remainingSessions',
      'expiresOn',
      'isUsable',
      'isRunningLow',
      'finishedOn',
    ],
  );
  final val = MePackage(
    id: $checkedConvert('id', (v) => v as String),
    brand: $checkedConvert(
      'brand',
      (v) => MeBrandRef.fromJson(v as Map<String, dynamic>),
    ),
    branchName: $checkedConvert('branchName', (v) => v as String?),
    isAvailableAllBranches: $checkedConvert(
      'isAvailableAllBranches',
      (v) => v as bool,
    ),
    packageName: $checkedConvert('packageName', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$CustomerPackageStatusEnumMap,
        v,
        unknownValue: CustomerPackageStatus.unknownDefaultOpenApi,
      ),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => CustomerPackageItemView.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    totalSessions: $checkedConvert('totalSessions', (v) => (v as num).toInt()),
    remainingSessions: $checkedConvert(
      'remainingSessions',
      (v) => (v as num).toInt(),
    ),
    expiresOn: $checkedConvert('expiresOn', (v) => DateTime.parse(v as String)),
    isUsable: $checkedConvert('isUsable', (v) => v as bool),
    isRunningLow: $checkedConvert('isRunningLow', (v) => v as bool),
    finishedOn: $checkedConvert(
      'finishedOn',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$MePackageToJson(MePackage instance) => <String, dynamic>{
  'id': instance.id,
  'brand': instance.brand.toJson(),
  'branchName': instance.branchName,
  'isAvailableAllBranches': instance.isAvailableAllBranches,
  'packageName': instance.packageName,
  'status': _$CustomerPackageStatusEnumMap[instance.status]!,
  'items': instance.items.map((e) => e.toJson()).toList(),
  'totalSessions': instance.totalSessions,
  'remainingSessions': instance.remainingSessions,
  'expiresOn': instance.expiresOn.toIso8601String(),
  'isUsable': instance.isUsable,
  'isRunningLow': instance.isRunningLow,
  'finishedOn': instance.finishedOn?.toIso8601String(),
};

const _$CustomerPackageStatusEnumMap = {
  CustomerPackageStatus.active: 'Active',
  CustomerPackageStatus.expired: 'Expired',
  CustomerPackageStatus.refunded: 'Refunded',
  CustomerPackageStatus.transferred: 'Transferred',
  CustomerPackageStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
