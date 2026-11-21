// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_package.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerPackageCWProxy {
  CustomerPackage id(String id);

  CustomerPackage customer(CustomerRef customer);

  CustomerPackage packageId(String packageId);

  CustomerPackage packageName(String packageName);

  CustomerPackage branchId(String branchId);

  CustomerPackage isAvailableAllBranches(bool isAvailableAllBranches);

  CustomerPackage price(Money price);

  CustomerPackage purchasedUtc(DateTime purchasedUtc);

  CustomerPackage expiresOn(DateTime expiresOn);

  CustomerPackage soldByStaffId(String? soldByStaffId);

  CustomerPackage soldByStaffName(String? soldByStaffName);

  CustomerPackage status(CustomerPackageStatus status);

  CustomerPackage items(List<CustomerPackageItemView> items);

  CustomerPackage totalSessions(int totalSessions);

  CustomerPackage remainingSessions(int remainingSessions);

  CustomerPackage refundedAmount(Money? refundedAmount);

  CustomerPackage refundedUtc(DateTime? refundedUtc);

  CustomerPackage usages(List<PackageSessionUsageView>? usages);

  CustomerPackage version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPackage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPackage call({
    String id,
    CustomerRef customer,
    String packageId,
    String packageName,
    String branchId,
    bool isAvailableAllBranches,
    Money price,
    DateTime purchasedUtc,
    DateTime expiresOn,
    String? soldByStaffId,
    String? soldByStaffName,
    CustomerPackageStatus status,
    List<CustomerPackageItemView> items,
    int totalSessions,
    int remainingSessions,
    Money? refundedAmount,
    DateTime? refundedUtc,
    List<PackageSessionUsageView>? usages,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerPackage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerPackage.copyWith.fieldName(...)`
class _$CustomerPackageCWProxyImpl implements _$CustomerPackageCWProxy {
  const _$CustomerPackageCWProxyImpl(this._value);

  final CustomerPackage _value;

  @override
  CustomerPackage id(String id) => this(id: id);

  @override
  CustomerPackage customer(CustomerRef customer) => this(customer: customer);

  @override
  CustomerPackage packageId(String packageId) => this(packageId: packageId);

  @override
  CustomerPackage packageName(String packageName) =>
      this(packageName: packageName);

  @override
  CustomerPackage branchId(String branchId) => this(branchId: branchId);

  @override
  CustomerPackage isAvailableAllBranches(bool isAvailableAllBranches) =>
      this(isAvailableAllBranches: isAvailableAllBranches);

  @override
  CustomerPackage price(Money price) => this(price: price);

  @override
  CustomerPackage purchasedUtc(DateTime purchasedUtc) =>
      this(purchasedUtc: purchasedUtc);

  @override
  CustomerPackage expiresOn(DateTime expiresOn) => this(expiresOn: expiresOn);

  @override
  CustomerPackage soldByStaffId(String? soldByStaffId) =>
      this(soldByStaffId: soldByStaffId);

  @override
  CustomerPackage soldByStaffName(String? soldByStaffName) =>
      this(soldByStaffName: soldByStaffName);

  @override
  CustomerPackage status(CustomerPackageStatus status) => this(status: status);

  @override
  CustomerPackage items(List<CustomerPackageItemView> items) =>
      this(items: items);

  @override
  CustomerPackage totalSessions(int totalSessions) =>
      this(totalSessions: totalSessions);

  @override
  CustomerPackage remainingSessions(int remainingSessions) =>
      this(remainingSessions: remainingSessions);

  @override
  CustomerPackage refundedAmount(Money? refundedAmount) =>
      this(refundedAmount: refundedAmount);

  @override
  CustomerPackage refundedUtc(DateTime? refundedUtc) =>
      this(refundedUtc: refundedUtc);

  @override
  CustomerPackage usages(List<PackageSessionUsageView>? usages) =>
      this(usages: usages);

  @override
  CustomerPackage version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPackage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPackage call({
    Object? id = const $CopyWithPlaceholder(),
    Object? customer = const $CopyWithPlaceholder(),
    Object? packageId = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? isAvailableAllBranches = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? purchasedUtc = const $CopyWithPlaceholder(),
    Object? expiresOn = const $CopyWithPlaceholder(),
    Object? soldByStaffId = const $CopyWithPlaceholder(),
    Object? soldByStaffName = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? totalSessions = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
    Object? refundedAmount = const $CopyWithPlaceholder(),
    Object? refundedUtc = const $CopyWithPlaceholder(),
    Object? usages = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return CustomerPackage(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      customer: customer == const $CopyWithPlaceholder()
          ? _value.customer
          // ignore: cast_nullable_to_non_nullable
          : customer as CustomerRef,
      packageId: packageId == const $CopyWithPlaceholder()
          ? _value.packageId
          // ignore: cast_nullable_to_non_nullable
          : packageId as String,
      packageName: packageName == const $CopyWithPlaceholder()
          ? _value.packageName
          // ignore: cast_nullable_to_non_nullable
          : packageName as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      isAvailableAllBranches:
          isAvailableAllBranches == const $CopyWithPlaceholder()
          ? _value.isAvailableAllBranches
          // ignore: cast_nullable_to_non_nullable
          : isAvailableAllBranches as bool,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
      purchasedUtc: purchasedUtc == const $CopyWithPlaceholder()
          ? _value.purchasedUtc
          // ignore: cast_nullable_to_non_nullable
          : purchasedUtc as DateTime,
      expiresOn: expiresOn == const $CopyWithPlaceholder()
          ? _value.expiresOn
          // ignore: cast_nullable_to_non_nullable
          : expiresOn as DateTime,
      soldByStaffId: soldByStaffId == const $CopyWithPlaceholder()
          ? _value.soldByStaffId
          // ignore: cast_nullable_to_non_nullable
          : soldByStaffId as String?,
      soldByStaffName: soldByStaffName == const $CopyWithPlaceholder()
          ? _value.soldByStaffName
          // ignore: cast_nullable_to_non_nullable
          : soldByStaffName as String?,
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
      refundedAmount: refundedAmount == const $CopyWithPlaceholder()
          ? _value.refundedAmount
          // ignore: cast_nullable_to_non_nullable
          : refundedAmount as Money?,
      refundedUtc: refundedUtc == const $CopyWithPlaceholder()
          ? _value.refundedUtc
          // ignore: cast_nullable_to_non_nullable
          : refundedUtc as DateTime?,
      usages: usages == const $CopyWithPlaceholder()
          ? _value.usages
          // ignore: cast_nullable_to_non_nullable
          : usages as List<PackageSessionUsageView>?,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $CustomerPackageCopyWith on CustomerPackage {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerPackage.copyWith(...)` or like so:`instanceOfCustomerPackage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerPackageCWProxy get copyWith => _$CustomerPackageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerPackage _$CustomerPackageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerPackage', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'customer',
      'packageId',
      'packageName',
      'branchId',
      'isAvailableAllBranches',
      'price',
      'purchasedUtc',
      'expiresOn',
      'status',
      'items',
      'totalSessions',
      'remainingSessions',
      'version',
    ],
  );
  final val = CustomerPackage(
    id: $checkedConvert('id', (v) => v as String),
    customer: $checkedConvert(
      'customer',
      (v) => CustomerRef.fromJson(v as Map<String, dynamic>),
    ),
    packageId: $checkedConvert('packageId', (v) => v as String),
    packageName: $checkedConvert('packageName', (v) => v as String),
    branchId: $checkedConvert('branchId', (v) => v as String),
    isAvailableAllBranches: $checkedConvert(
      'isAvailableAllBranches',
      (v) => v as bool,
    ),
    price: $checkedConvert(
      'price',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    purchasedUtc: $checkedConvert(
      'purchasedUtc',
      (v) => DateTime.parse(v as String),
    ),
    expiresOn: $checkedConvert('expiresOn', (v) => DateTime.parse(v as String)),
    soldByStaffId: $checkedConvert('soldByStaffId', (v) => v as String?),
    soldByStaffName: $checkedConvert('soldByStaffName', (v) => v as String?),
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
    refundedAmount: $checkedConvert(
      'refundedAmount',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    refundedUtc: $checkedConvert(
      'refundedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    usages: $checkedConvert(
      'usages',
      (v) => (v as List<dynamic>?)
          ?.map(
            (e) => PackageSessionUsageView.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$CustomerPackageToJson(CustomerPackage instance) =>
    <String, dynamic>{
      'id': instance.id,
      'customer': instance.customer.toJson(),
      'packageId': instance.packageId,
      'packageName': instance.packageName,
      'branchId': instance.branchId,
      'isAvailableAllBranches': instance.isAvailableAllBranches,
      'price': instance.price.toJson(),
      'purchasedUtc': instance.purchasedUtc.toIso8601String(),
      'expiresOn': instance.expiresOn.toIso8601String(),
      'soldByStaffId': ?instance.soldByStaffId,
      'soldByStaffName': ?instance.soldByStaffName,
      'status': _$CustomerPackageStatusEnumMap[instance.status]!,
      'items': instance.items.map((e) => e.toJson()).toList(),
      'totalSessions': instance.totalSessions,
      'remainingSessions': instance.remainingSessions,
      'refundedAmount': ?instance.refundedAmount?.toJson(),
      'refundedUtc': ?instance.refundedUtc?.toIso8601String(),
      'usages': ?instance.usages?.map((e) => e.toJson()).toList(),
      'version': instance.version,
    };

const _$CustomerPackageStatusEnumMap = {
  CustomerPackageStatus.active: 'Active',
  CustomerPackageStatus.expired: 'Expired',
  CustomerPackageStatus.refunded: 'Refunded',
  CustomerPackageStatus.transferred: 'Transferred',
  CustomerPackageStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
