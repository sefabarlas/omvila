// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_overview_weekly_occupancy.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageOverviewWeeklyOccupancyCWProxy {
  ManageOverviewWeeklyOccupancy weekStart(DateTime weekStart);

  ManageOverviewWeeklyOccupancy percent(int? percent);

  ManageOverviewWeeklyOccupancy previousWeekPercent(int? previousWeekPercent);

  ManageOverviewWeeklyOccupancy staff(
    List<ManageOverviewWeeklyOccupancyStaffInner> staff,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewWeeklyOccupancy(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewWeeklyOccupancy(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewWeeklyOccupancy call({
    DateTime weekStart,
    int? percent,
    int? previousWeekPercent,
    List<ManageOverviewWeeklyOccupancyStaffInner> staff,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageOverviewWeeklyOccupancy.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageOverviewWeeklyOccupancy.copyWith.fieldName(...)`
class _$ManageOverviewWeeklyOccupancyCWProxyImpl
    implements _$ManageOverviewWeeklyOccupancyCWProxy {
  const _$ManageOverviewWeeklyOccupancyCWProxyImpl(this._value);

  final ManageOverviewWeeklyOccupancy _value;

  @override
  ManageOverviewWeeklyOccupancy weekStart(DateTime weekStart) =>
      this(weekStart: weekStart);

  @override
  ManageOverviewWeeklyOccupancy percent(int? percent) => this(percent: percent);

  @override
  ManageOverviewWeeklyOccupancy previousWeekPercent(int? previousWeekPercent) =>
      this(previousWeekPercent: previousWeekPercent);

  @override
  ManageOverviewWeeklyOccupancy staff(
    List<ManageOverviewWeeklyOccupancyStaffInner> staff,
  ) => this(staff: staff);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewWeeklyOccupancy(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewWeeklyOccupancy(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewWeeklyOccupancy call({
    Object? weekStart = const $CopyWithPlaceholder(),
    Object? percent = const $CopyWithPlaceholder(),
    Object? previousWeekPercent = const $CopyWithPlaceholder(),
    Object? staff = const $CopyWithPlaceholder(),
  }) {
    return ManageOverviewWeeklyOccupancy(
      weekStart: weekStart == const $CopyWithPlaceholder()
          ? _value.weekStart
          // ignore: cast_nullable_to_non_nullable
          : weekStart as DateTime,
      percent: percent == const $CopyWithPlaceholder()
          ? _value.percent
          // ignore: cast_nullable_to_non_nullable
          : percent as int?,
      previousWeekPercent: previousWeekPercent == const $CopyWithPlaceholder()
          ? _value.previousWeekPercent
          // ignore: cast_nullable_to_non_nullable
          : previousWeekPercent as int?,
      staff: staff == const $CopyWithPlaceholder()
          ? _value.staff
          // ignore: cast_nullable_to_non_nullable
          : staff as List<ManageOverviewWeeklyOccupancyStaffInner>,
    );
  }
}

extension $ManageOverviewWeeklyOccupancyCopyWith
    on ManageOverviewWeeklyOccupancy {
  /// Returns a callable class that can be used as follows: `instanceOfManageOverviewWeeklyOccupancy.copyWith(...)` or like so:`instanceOfManageOverviewWeeklyOccupancy.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageOverviewWeeklyOccupancyCWProxy get copyWith =>
      _$ManageOverviewWeeklyOccupancyCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageOverviewWeeklyOccupancy _$ManageOverviewWeeklyOccupancyFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageOverviewWeeklyOccupancy', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'weekStart',
      'percent',
      'previousWeekPercent',
      'staff',
    ],
  );
  final val = ManageOverviewWeeklyOccupancy(
    weekStart: $checkedConvert(
      'weekStart',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    percent: $checkedConvert('percent', (v) => (v as num?)?.toInt()),
    previousWeekPercent: $checkedConvert(
      'previousWeekPercent',
      (v) => (v as num?)?.toInt(),
    ),
    staff: $checkedConvert(
      'staff',
      (v) => (v as List<dynamic>)
          .map(
            (e) => ManageOverviewWeeklyOccupancyStaffInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageOverviewWeeklyOccupancyToJson(
  ManageOverviewWeeklyOccupancy instance,
) => <String, dynamic>{
  'weekStart': const DateOnlyConverter().toJson(instance.weekStart),
  'percent': instance.percent,
  'previousWeekPercent': instance.previousWeekPercent,
  'staff': instance.staff.map((e) => e.toJson()).toList(),
};
