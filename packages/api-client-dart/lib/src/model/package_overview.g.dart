// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageOverviewCWProxy {
  PackageOverview activeCustomerPackages(int activeCustomerPackages);

  PackageOverview remainingSessions(int remainingSessions);

  PackageOverview soldThisMonth(int soldThisMonth);

  PackageOverview soldThisMonthAmount(Money soldThisMonthAmount);

  PackageOverview expiringSoon(int expiringSoon);

  PackageOverview usageRatePercent(num? usageRatePercent);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageOverview call({
    int activeCustomerPackages,
    int remainingSessions,
    int soldThisMonth,
    Money soldThisMonthAmount,
    int expiringSoon,
    num? usageRatePercent,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageOverview.copyWith.fieldName(...)`
class _$PackageOverviewCWProxyImpl implements _$PackageOverviewCWProxy {
  const _$PackageOverviewCWProxyImpl(this._value);

  final PackageOverview _value;

  @override
  PackageOverview activeCustomerPackages(int activeCustomerPackages) =>
      this(activeCustomerPackages: activeCustomerPackages);

  @override
  PackageOverview remainingSessions(int remainingSessions) =>
      this(remainingSessions: remainingSessions);

  @override
  PackageOverview soldThisMonth(int soldThisMonth) =>
      this(soldThisMonth: soldThisMonth);

  @override
  PackageOverview soldThisMonthAmount(Money soldThisMonthAmount) =>
      this(soldThisMonthAmount: soldThisMonthAmount);

  @override
  PackageOverview expiringSoon(int expiringSoon) =>
      this(expiringSoon: expiringSoon);

  @override
  PackageOverview usageRatePercent(num? usageRatePercent) =>
      this(usageRatePercent: usageRatePercent);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageOverview call({
    Object? activeCustomerPackages = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
    Object? soldThisMonth = const $CopyWithPlaceholder(),
    Object? soldThisMonthAmount = const $CopyWithPlaceholder(),
    Object? expiringSoon = const $CopyWithPlaceholder(),
    Object? usageRatePercent = const $CopyWithPlaceholder(),
  }) {
    return PackageOverview(
      activeCustomerPackages:
          activeCustomerPackages == const $CopyWithPlaceholder()
          ? _value.activeCustomerPackages
          // ignore: cast_nullable_to_non_nullable
          : activeCustomerPackages as int,
      remainingSessions: remainingSessions == const $CopyWithPlaceholder()
          ? _value.remainingSessions
          // ignore: cast_nullable_to_non_nullable
          : remainingSessions as int,
      soldThisMonth: soldThisMonth == const $CopyWithPlaceholder()
          ? _value.soldThisMonth
          // ignore: cast_nullable_to_non_nullable
          : soldThisMonth as int,
      soldThisMonthAmount: soldThisMonthAmount == const $CopyWithPlaceholder()
          ? _value.soldThisMonthAmount
          // ignore: cast_nullable_to_non_nullable
          : soldThisMonthAmount as Money,
      expiringSoon: expiringSoon == const $CopyWithPlaceholder()
          ? _value.expiringSoon
          // ignore: cast_nullable_to_non_nullable
          : expiringSoon as int,
      usageRatePercent: usageRatePercent == const $CopyWithPlaceholder()
          ? _value.usageRatePercent
          // ignore: cast_nullable_to_non_nullable
          : usageRatePercent as num?,
    );
  }
}

extension $PackageOverviewCopyWith on PackageOverview {
  /// Returns a callable class that can be used as follows: `instanceOfPackageOverview.copyWith(...)` or like so:`instanceOfPackageOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageOverviewCWProxy get copyWith => _$PackageOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageOverview _$PackageOverviewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PackageOverview', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'activeCustomerPackages',
      'remainingSessions',
      'soldThisMonth',
      'soldThisMonthAmount',
      'expiringSoon',
    ],
  );
  final val = PackageOverview(
    activeCustomerPackages: $checkedConvert(
      'activeCustomerPackages',
      (v) => (v as num).toInt(),
    ),
    remainingSessions: $checkedConvert(
      'remainingSessions',
      (v) => (v as num).toInt(),
    ),
    soldThisMonth: $checkedConvert('soldThisMonth', (v) => (v as num).toInt()),
    soldThisMonthAmount: $checkedConvert(
      'soldThisMonthAmount',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    expiringSoon: $checkedConvert('expiringSoon', (v) => (v as num).toInt()),
    usageRatePercent: $checkedConvert('usageRatePercent', (v) => v as num?),
  );
  return val;
});

Map<String, dynamic> _$PackageOverviewToJson(PackageOverview instance) =>
    <String, dynamic>{
      'activeCustomerPackages': instance.activeCustomerPackages,
      'remainingSessions': instance.remainingSessions,
      'soldThisMonth': instance.soldThisMonth,
      'soldThisMonthAmount': instance.soldThisMonthAmount.toJson(),
      'expiringSoon': instance.expiringSoon,
      'usageRatePercent': ?instance.usageRatePercent,
    };
