// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calendar_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CalendarViewCWProxy {
  CalendarView branchId(String branchId);

  CalendarView timeZone(String timeZone);

  CalendarView from(DateTime from);

  CalendarView to(DateTime to);

  CalendarView staff(List<CalendarViewStaffInner> staff);

  CalendarView days(List<CalendarViewDaysInner> days);

  CalendarView appointments(List<CalendarAppointment> appointments);

  CalendarView timeOffs(List<TimeOff> timeOffs);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarView(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarView call({
    String branchId,
    String timeZone,
    DateTime from,
    DateTime to,
    List<CalendarViewStaffInner> staff,
    List<CalendarViewDaysInner> days,
    List<CalendarAppointment> appointments,
    List<TimeOff> timeOffs,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCalendarView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCalendarView.copyWith.fieldName(...)`
class _$CalendarViewCWProxyImpl implements _$CalendarViewCWProxy {
  const _$CalendarViewCWProxyImpl(this._value);

  final CalendarView _value;

  @override
  CalendarView branchId(String branchId) => this(branchId: branchId);

  @override
  CalendarView timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  CalendarView from(DateTime from) => this(from: from);

  @override
  CalendarView to(DateTime to) => this(to: to);

  @override
  CalendarView staff(List<CalendarViewStaffInner> staff) => this(staff: staff);

  @override
  CalendarView days(List<CalendarViewDaysInner> days) => this(days: days);

  @override
  CalendarView appointments(List<CalendarAppointment> appointments) =>
      this(appointments: appointments);

  @override
  CalendarView timeOffs(List<TimeOff> timeOffs) => this(timeOffs: timeOffs);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarView(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarView call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? from = const $CopyWithPlaceholder(),
    Object? to = const $CopyWithPlaceholder(),
    Object? staff = const $CopyWithPlaceholder(),
    Object? days = const $CopyWithPlaceholder(),
    Object? appointments = const $CopyWithPlaceholder(),
    Object? timeOffs = const $CopyWithPlaceholder(),
  }) {
    return CalendarView(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      from: from == const $CopyWithPlaceholder()
          ? _value.from
          // ignore: cast_nullable_to_non_nullable
          : from as DateTime,
      to: to == const $CopyWithPlaceholder()
          ? _value.to
          // ignore: cast_nullable_to_non_nullable
          : to as DateTime,
      staff: staff == const $CopyWithPlaceholder()
          ? _value.staff
          // ignore: cast_nullable_to_non_nullable
          : staff as List<CalendarViewStaffInner>,
      days: days == const $CopyWithPlaceholder()
          ? _value.days
          // ignore: cast_nullable_to_non_nullable
          : days as List<CalendarViewDaysInner>,
      appointments: appointments == const $CopyWithPlaceholder()
          ? _value.appointments
          // ignore: cast_nullable_to_non_nullable
          : appointments as List<CalendarAppointment>,
      timeOffs: timeOffs == const $CopyWithPlaceholder()
          ? _value.timeOffs
          // ignore: cast_nullable_to_non_nullable
          : timeOffs as List<TimeOff>,
    );
  }
}

extension $CalendarViewCopyWith on CalendarView {
  /// Returns a callable class that can be used as follows: `instanceOfCalendarView.copyWith(...)` or like so:`instanceOfCalendarView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CalendarViewCWProxy get copyWith => _$CalendarViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CalendarView _$CalendarViewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CalendarView', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'branchId',
      'timeZone',
      'from',
      'to',
      'staff',
      'days',
      'appointments',
      'timeOffs',
    ],
  );
  final val = CalendarView(
    branchId: $checkedConvert('branchId', (v) => v as String),
    timeZone: $checkedConvert('timeZone', (v) => v as String),
    from: $checkedConvert(
      'from',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    to: $checkedConvert(
      'to',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    staff: $checkedConvert(
      'staff',
      (v) => (v as List<dynamic>)
          .map(
            (e) => CalendarViewStaffInner.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    days: $checkedConvert(
      'days',
      (v) => (v as List<dynamic>)
          .map((e) => CalendarViewDaysInner.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    appointments: $checkedConvert(
      'appointments',
      (v) => (v as List<dynamic>)
          .map((e) => CalendarAppointment.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    timeOffs: $checkedConvert(
      'timeOffs',
      (v) => (v as List<dynamic>)
          .map((e) => TimeOff.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$CalendarViewToJson(CalendarView instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'timeZone': instance.timeZone,
      'from': const DateOnlyConverter().toJson(instance.from),
      'to': const DateOnlyConverter().toJson(instance.to),
      'staff': instance.staff.map((e) => e.toJson()).toList(),
      'days': instance.days.map((e) => e.toJson()).toList(),
      'appointments': instance.appointments.map((e) => e.toJson()).toList(),
      'timeOffs': instance.timeOffs.map((e) => e.toJson()).toList(),
    };
