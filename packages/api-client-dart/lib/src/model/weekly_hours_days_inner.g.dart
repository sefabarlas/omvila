// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'weekly_hours_days_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WeeklyHoursDaysInnerCWProxy {
  WeeklyHoursDaysInner dayOfWeek(int dayOfWeek);

  WeeklyHoursDaysInner intervals(List<TimeInterval> intervals);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WeeklyHoursDaysInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WeeklyHoursDaysInner(...).copyWith(id: 12, name: "My name")
  /// ````
  WeeklyHoursDaysInner call({int dayOfWeek, List<TimeInterval> intervals});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWeeklyHoursDaysInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWeeklyHoursDaysInner.copyWith.fieldName(...)`
class _$WeeklyHoursDaysInnerCWProxyImpl
    implements _$WeeklyHoursDaysInnerCWProxy {
  const _$WeeklyHoursDaysInnerCWProxyImpl(this._value);

  final WeeklyHoursDaysInner _value;

  @override
  WeeklyHoursDaysInner dayOfWeek(int dayOfWeek) => this(dayOfWeek: dayOfWeek);

  @override
  WeeklyHoursDaysInner intervals(List<TimeInterval> intervals) =>
      this(intervals: intervals);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WeeklyHoursDaysInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WeeklyHoursDaysInner(...).copyWith(id: 12, name: "My name")
  /// ````
  WeeklyHoursDaysInner call({
    Object? dayOfWeek = const $CopyWithPlaceholder(),
    Object? intervals = const $CopyWithPlaceholder(),
  }) {
    return WeeklyHoursDaysInner(
      dayOfWeek: dayOfWeek == const $CopyWithPlaceholder()
          ? _value.dayOfWeek
          // ignore: cast_nullable_to_non_nullable
          : dayOfWeek as int,
      intervals: intervals == const $CopyWithPlaceholder()
          ? _value.intervals
          // ignore: cast_nullable_to_non_nullable
          : intervals as List<TimeInterval>,
    );
  }
}

extension $WeeklyHoursDaysInnerCopyWith on WeeklyHoursDaysInner {
  /// Returns a callable class that can be used as follows: `instanceOfWeeklyHoursDaysInner.copyWith(...)` or like so:`instanceOfWeeklyHoursDaysInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WeeklyHoursDaysInnerCWProxy get copyWith =>
      _$WeeklyHoursDaysInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WeeklyHoursDaysInner _$WeeklyHoursDaysInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('WeeklyHoursDaysInner', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['dayOfWeek', 'intervals']);
  final val = WeeklyHoursDaysInner(
    dayOfWeek: $checkedConvert('dayOfWeek', (v) => (v as num).toInt()),
    intervals: $checkedConvert(
      'intervals',
      (v) => (v as List<dynamic>)
          .map((e) => TimeInterval.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$WeeklyHoursDaysInnerToJson(
  WeeklyHoursDaysInner instance,
) => <String, dynamic>{
  'dayOfWeek': instance.dayOfWeek,
  'intervals': instance.intervals.map((e) => e.toJson()).toList(),
};
