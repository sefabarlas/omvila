// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calendar_view_days_inner_staff_hours_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CalendarViewDaysInnerStaffHoursInnerCWProxy {
  CalendarViewDaysInnerStaffHoursInner staffId(String staffId);

  CalendarViewDaysInnerStaffHoursInner intervals(List<TimeInterval> intervals);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarViewDaysInnerStaffHoursInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarViewDaysInnerStaffHoursInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarViewDaysInnerStaffHoursInner call({
    String staffId,
    List<TimeInterval> intervals,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCalendarViewDaysInnerStaffHoursInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCalendarViewDaysInnerStaffHoursInner.copyWith.fieldName(...)`
class _$CalendarViewDaysInnerStaffHoursInnerCWProxyImpl
    implements _$CalendarViewDaysInnerStaffHoursInnerCWProxy {
  const _$CalendarViewDaysInnerStaffHoursInnerCWProxyImpl(this._value);

  final CalendarViewDaysInnerStaffHoursInner _value;

  @override
  CalendarViewDaysInnerStaffHoursInner staffId(String staffId) =>
      this(staffId: staffId);

  @override
  CalendarViewDaysInnerStaffHoursInner intervals(
    List<TimeInterval> intervals,
  ) => this(intervals: intervals);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarViewDaysInnerStaffHoursInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarViewDaysInnerStaffHoursInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarViewDaysInnerStaffHoursInner call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? intervals = const $CopyWithPlaceholder(),
  }) {
    return CalendarViewDaysInnerStaffHoursInner(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      intervals: intervals == const $CopyWithPlaceholder()
          ? _value.intervals
          // ignore: cast_nullable_to_non_nullable
          : intervals as List<TimeInterval>,
    );
  }
}

extension $CalendarViewDaysInnerStaffHoursInnerCopyWith
    on CalendarViewDaysInnerStaffHoursInner {
  /// Returns a callable class that can be used as follows: `instanceOfCalendarViewDaysInnerStaffHoursInner.copyWith(...)` or like so:`instanceOfCalendarViewDaysInnerStaffHoursInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CalendarViewDaysInnerStaffHoursInnerCWProxy get copyWith =>
      _$CalendarViewDaysInnerStaffHoursInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CalendarViewDaysInnerStaffHoursInner
_$CalendarViewDaysInnerStaffHoursInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CalendarViewDaysInnerStaffHoursInner', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['staffId', 'intervals']);
      final val = CalendarViewDaysInnerStaffHoursInner(
        staffId: $checkedConvert('staffId', (v) => v as String),
        intervals: $checkedConvert(
          'intervals',
          (v) => (v as List<dynamic>)
              .map((e) => TimeInterval.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CalendarViewDaysInnerStaffHoursInnerToJson(
  CalendarViewDaysInnerStaffHoursInner instance,
) => <String, dynamic>{
  'staffId': instance.staffId,
  'intervals': instance.intervals.map((e) => e.toJson()).toList(),
};
