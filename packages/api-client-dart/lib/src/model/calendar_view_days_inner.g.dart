// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calendar_view_days_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CalendarViewDaysInnerCWProxy {
  CalendarViewDaysInner date(DateTime date);

  CalendarViewDaysInner isClosed(bool isClosed);

  CalendarViewDaysInner closureNote(String? closureNote);

  CalendarViewDaysInner branchIntervals(List<TimeInterval>? branchIntervals);

  CalendarViewDaysInner staffHours(
    List<CalendarViewDaysInnerStaffHoursInner> staffHours,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarViewDaysInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarViewDaysInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarViewDaysInner call({
    DateTime date,
    bool isClosed,
    String? closureNote,
    List<TimeInterval>? branchIntervals,
    List<CalendarViewDaysInnerStaffHoursInner> staffHours,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCalendarViewDaysInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCalendarViewDaysInner.copyWith.fieldName(...)`
class _$CalendarViewDaysInnerCWProxyImpl
    implements _$CalendarViewDaysInnerCWProxy {
  const _$CalendarViewDaysInnerCWProxyImpl(this._value);

  final CalendarViewDaysInner _value;

  @override
  CalendarViewDaysInner date(DateTime date) => this(date: date);

  @override
  CalendarViewDaysInner isClosed(bool isClosed) => this(isClosed: isClosed);

  @override
  CalendarViewDaysInner closureNote(String? closureNote) =>
      this(closureNote: closureNote);

  @override
  CalendarViewDaysInner branchIntervals(List<TimeInterval>? branchIntervals) =>
      this(branchIntervals: branchIntervals);

  @override
  CalendarViewDaysInner staffHours(
    List<CalendarViewDaysInnerStaffHoursInner> staffHours,
  ) => this(staffHours: staffHours);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarViewDaysInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarViewDaysInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarViewDaysInner call({
    Object? date = const $CopyWithPlaceholder(),
    Object? isClosed = const $CopyWithPlaceholder(),
    Object? closureNote = const $CopyWithPlaceholder(),
    Object? branchIntervals = const $CopyWithPlaceholder(),
    Object? staffHours = const $CopyWithPlaceholder(),
  }) {
    return CalendarViewDaysInner(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      isClosed: isClosed == const $CopyWithPlaceholder()
          ? _value.isClosed
          // ignore: cast_nullable_to_non_nullable
          : isClosed as bool,
      closureNote: closureNote == const $CopyWithPlaceholder()
          ? _value.closureNote
          // ignore: cast_nullable_to_non_nullable
          : closureNote as String?,
      branchIntervals: branchIntervals == const $CopyWithPlaceholder()
          ? _value.branchIntervals
          // ignore: cast_nullable_to_non_nullable
          : branchIntervals as List<TimeInterval>?,
      staffHours: staffHours == const $CopyWithPlaceholder()
          ? _value.staffHours
          // ignore: cast_nullable_to_non_nullable
          : staffHours as List<CalendarViewDaysInnerStaffHoursInner>,
    );
  }
}

extension $CalendarViewDaysInnerCopyWith on CalendarViewDaysInner {
  /// Returns a callable class that can be used as follows: `instanceOfCalendarViewDaysInner.copyWith(...)` or like so:`instanceOfCalendarViewDaysInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CalendarViewDaysInnerCWProxy get copyWith =>
      _$CalendarViewDaysInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CalendarViewDaysInner _$CalendarViewDaysInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CalendarViewDaysInner', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['date', 'isClosed', 'staffHours']);
  final val = CalendarViewDaysInner(
    date: $checkedConvert(
      'date',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    isClosed: $checkedConvert('isClosed', (v) => v as bool),
    closureNote: $checkedConvert('closureNote', (v) => v as String?),
    branchIntervals: $checkedConvert(
      'branchIntervals',
      (v) => (v as List<dynamic>?)
          ?.map((e) => TimeInterval.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    staffHours: $checkedConvert(
      'staffHours',
      (v) => (v as List<dynamic>)
          .map(
            (e) => CalendarViewDaysInnerStaffHoursInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$CalendarViewDaysInnerToJson(
  CalendarViewDaysInner instance,
) => <String, dynamic>{
  'date': const DateOnlyConverter().toJson(instance.date),
  'isClosed': instance.isClosed,
  'closureNote': ?instance.closureNote,
  'branchIntervals': ?instance.branchIntervals?.map((e) => e.toJson()).toList(),
  'staffHours': instance.staffHours.map((e) => e.toJson()).toList(),
};
