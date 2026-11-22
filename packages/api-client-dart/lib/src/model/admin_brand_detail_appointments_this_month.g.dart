// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_detail_appointments_this_month.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandDetailAppointmentsThisMonthCWProxy {
  AdminBrandDetailAppointmentsThisMonth count(int count);

  AdminBrandDetailAppointmentsThisMonth onlinePercent(num? onlinePercent);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailAppointmentsThisMonth(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailAppointmentsThisMonth(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailAppointmentsThisMonth call({int count, num? onlinePercent});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandDetailAppointmentsThisMonth.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandDetailAppointmentsThisMonth.copyWith.fieldName(...)`
class _$AdminBrandDetailAppointmentsThisMonthCWProxyImpl
    implements _$AdminBrandDetailAppointmentsThisMonthCWProxy {
  const _$AdminBrandDetailAppointmentsThisMonthCWProxyImpl(this._value);

  final AdminBrandDetailAppointmentsThisMonth _value;

  @override
  AdminBrandDetailAppointmentsThisMonth count(int count) => this(count: count);

  @override
  AdminBrandDetailAppointmentsThisMonth onlinePercent(num? onlinePercent) =>
      this(onlinePercent: onlinePercent);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailAppointmentsThisMonth(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailAppointmentsThisMonth(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailAppointmentsThisMonth call({
    Object? count = const $CopyWithPlaceholder(),
    Object? onlinePercent = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandDetailAppointmentsThisMonth(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      onlinePercent: onlinePercent == const $CopyWithPlaceholder()
          ? _value.onlinePercent
          // ignore: cast_nullable_to_non_nullable
          : onlinePercent as num?,
    );
  }
}

extension $AdminBrandDetailAppointmentsThisMonthCopyWith
    on AdminBrandDetailAppointmentsThisMonth {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandDetailAppointmentsThisMonth.copyWith(...)` or like so:`instanceOfAdminBrandDetailAppointmentsThisMonth.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandDetailAppointmentsThisMonthCWProxy get copyWith =>
      _$AdminBrandDetailAppointmentsThisMonthCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandDetailAppointmentsThisMonth
_$AdminBrandDetailAppointmentsThisMonthFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminBrandDetailAppointmentsThisMonth', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['count', 'onlinePercent']);
      final val = AdminBrandDetailAppointmentsThisMonth(
        count: $checkedConvert('count', (v) => (v as num).toInt()),
        onlinePercent: $checkedConvert('onlinePercent', (v) => v as num?),
      );
      return val;
    });

Map<String, dynamic> _$AdminBrandDetailAppointmentsThisMonthToJson(
  AdminBrandDetailAppointmentsThisMonth instance,
) => <String, dynamic>{
  'count': instance.count,
  'onlinePercent': instance.onlinePercent,
};
