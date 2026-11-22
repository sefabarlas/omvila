// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_overview_active_brands.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOverviewActiveBrandsCWProxy {
  AdminOverviewActiveBrands count(int count);

  AdminOverviewActiveBrands newThisMonth(int newThisMonth);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewActiveBrands(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewActiveBrands(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewActiveBrands call({int count, int newThisMonth});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOverviewActiveBrands.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOverviewActiveBrands.copyWith.fieldName(...)`
class _$AdminOverviewActiveBrandsCWProxyImpl
    implements _$AdminOverviewActiveBrandsCWProxy {
  const _$AdminOverviewActiveBrandsCWProxyImpl(this._value);

  final AdminOverviewActiveBrands _value;

  @override
  AdminOverviewActiveBrands count(int count) => this(count: count);

  @override
  AdminOverviewActiveBrands newThisMonth(int newThisMonth) =>
      this(newThisMonth: newThisMonth);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewActiveBrands(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewActiveBrands(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewActiveBrands call({
    Object? count = const $CopyWithPlaceholder(),
    Object? newThisMonth = const $CopyWithPlaceholder(),
  }) {
    return AdminOverviewActiveBrands(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      newThisMonth: newThisMonth == const $CopyWithPlaceholder()
          ? _value.newThisMonth
          // ignore: cast_nullable_to_non_nullable
          : newThisMonth as int,
    );
  }
}

extension $AdminOverviewActiveBrandsCopyWith on AdminOverviewActiveBrands {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOverviewActiveBrands.copyWith(...)` or like so:`instanceOfAdminOverviewActiveBrands.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOverviewActiveBrandsCWProxy get copyWith =>
      _$AdminOverviewActiveBrandsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOverviewActiveBrands _$AdminOverviewActiveBrandsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminOverviewActiveBrands', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'newThisMonth']);
  final val = AdminOverviewActiveBrands(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    newThisMonth: $checkedConvert('newThisMonth', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminOverviewActiveBrandsToJson(
  AdminOverviewActiveBrands instance,
) => <String, dynamic>{
  'count': instance.count,
  'newThisMonth': instance.newThisMonth,
};
