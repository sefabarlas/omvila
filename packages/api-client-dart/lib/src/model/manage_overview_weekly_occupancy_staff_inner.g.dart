// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_overview_weekly_occupancy_staff_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageOverviewWeeklyOccupancyStaffInnerCWProxy {
  ManageOverviewWeeklyOccupancyStaffInner staffId(String staffId);

  ManageOverviewWeeklyOccupancyStaffInner staffName(String staffName);

  ManageOverviewWeeklyOccupancyStaffInner calendarColor(String calendarColor);

  ManageOverviewWeeklyOccupancyStaffInner percent(int percent);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewWeeklyOccupancyStaffInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewWeeklyOccupancyStaffInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewWeeklyOccupancyStaffInner call({
    String staffId,
    String staffName,
    String calendarColor,
    int percent,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageOverviewWeeklyOccupancyStaffInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageOverviewWeeklyOccupancyStaffInner.copyWith.fieldName(...)`
class _$ManageOverviewWeeklyOccupancyStaffInnerCWProxyImpl
    implements _$ManageOverviewWeeklyOccupancyStaffInnerCWProxy {
  const _$ManageOverviewWeeklyOccupancyStaffInnerCWProxyImpl(this._value);

  final ManageOverviewWeeklyOccupancyStaffInner _value;

  @override
  ManageOverviewWeeklyOccupancyStaffInner staffId(String staffId) =>
      this(staffId: staffId);

  @override
  ManageOverviewWeeklyOccupancyStaffInner staffName(String staffName) =>
      this(staffName: staffName);

  @override
  ManageOverviewWeeklyOccupancyStaffInner calendarColor(String calendarColor) =>
      this(calendarColor: calendarColor);

  @override
  ManageOverviewWeeklyOccupancyStaffInner percent(int percent) =>
      this(percent: percent);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewWeeklyOccupancyStaffInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewWeeklyOccupancyStaffInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewWeeklyOccupancyStaffInner call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? calendarColor = const $CopyWithPlaceholder(),
    Object? percent = const $CopyWithPlaceholder(),
  }) {
    return ManageOverviewWeeklyOccupancyStaffInner(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String,
      calendarColor: calendarColor == const $CopyWithPlaceholder()
          ? _value.calendarColor
          // ignore: cast_nullable_to_non_nullable
          : calendarColor as String,
      percent: percent == const $CopyWithPlaceholder()
          ? _value.percent
          // ignore: cast_nullable_to_non_nullable
          : percent as int,
    );
  }
}

extension $ManageOverviewWeeklyOccupancyStaffInnerCopyWith
    on ManageOverviewWeeklyOccupancyStaffInner {
  /// Returns a callable class that can be used as follows: `instanceOfManageOverviewWeeklyOccupancyStaffInner.copyWith(...)` or like so:`instanceOfManageOverviewWeeklyOccupancyStaffInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageOverviewWeeklyOccupancyStaffInnerCWProxy get copyWith =>
      _$ManageOverviewWeeklyOccupancyStaffInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageOverviewWeeklyOccupancyStaffInner
_$ManageOverviewWeeklyOccupancyStaffInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageOverviewWeeklyOccupancyStaffInner', json, (
      $checkedConvert,
    ) {
      $checkKeys(
        json,
        requiredKeys: const [
          'staffId',
          'staffName',
          'calendarColor',
          'percent',
        ],
      );
      final val = ManageOverviewWeeklyOccupancyStaffInner(
        staffId: $checkedConvert('staffId', (v) => v as String),
        staffName: $checkedConvert('staffName', (v) => v as String),
        calendarColor: $checkedConvert('calendarColor', (v) => v as String),
        percent: $checkedConvert('percent', (v) => (v as num).toInt()),
      );
      return val;
    });

Map<String, dynamic> _$ManageOverviewWeeklyOccupancyStaffInnerToJson(
  ManageOverviewWeeklyOccupancyStaffInner instance,
) => <String, dynamic>{
  'staffId': instance.staffId,
  'staffName': instance.staffName,
  'calendarColor': instance.calendarColor,
  'percent': instance.percent,
};
