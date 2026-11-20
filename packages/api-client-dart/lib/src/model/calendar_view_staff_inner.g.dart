// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calendar_view_staff_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CalendarViewStaffInnerCWProxy {
  CalendarViewStaffInner staffId(String staffId);

  CalendarViewStaffInner fullName(String fullName);

  CalendarViewStaffInner calendarColor(String calendarColor);

  CalendarViewStaffInner photoUrl(String? photoUrl);

  CalendarViewStaffInner appointmentCount(int appointmentCount);

  CalendarViewStaffInner hasGapAvailability(bool hasGapAvailability);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarViewStaffInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarViewStaffInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarViewStaffInner call({
    String staffId,
    String fullName,
    String calendarColor,
    String? photoUrl,
    int appointmentCount,
    bool hasGapAvailability,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCalendarViewStaffInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCalendarViewStaffInner.copyWith.fieldName(...)`
class _$CalendarViewStaffInnerCWProxyImpl
    implements _$CalendarViewStaffInnerCWProxy {
  const _$CalendarViewStaffInnerCWProxyImpl(this._value);

  final CalendarViewStaffInner _value;

  @override
  CalendarViewStaffInner staffId(String staffId) => this(staffId: staffId);

  @override
  CalendarViewStaffInner fullName(String fullName) => this(fullName: fullName);

  @override
  CalendarViewStaffInner calendarColor(String calendarColor) =>
      this(calendarColor: calendarColor);

  @override
  CalendarViewStaffInner photoUrl(String? photoUrl) => this(photoUrl: photoUrl);

  @override
  CalendarViewStaffInner appointmentCount(int appointmentCount) =>
      this(appointmentCount: appointmentCount);

  @override
  CalendarViewStaffInner hasGapAvailability(bool hasGapAvailability) =>
      this(hasGapAvailability: hasGapAvailability);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarViewStaffInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarViewStaffInner(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarViewStaffInner call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? calendarColor = const $CopyWithPlaceholder(),
    Object? photoUrl = const $CopyWithPlaceholder(),
    Object? appointmentCount = const $CopyWithPlaceholder(),
    Object? hasGapAvailability = const $CopyWithPlaceholder(),
  }) {
    return CalendarViewStaffInner(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      calendarColor: calendarColor == const $CopyWithPlaceholder()
          ? _value.calendarColor
          // ignore: cast_nullable_to_non_nullable
          : calendarColor as String,
      photoUrl: photoUrl == const $CopyWithPlaceholder()
          ? _value.photoUrl
          // ignore: cast_nullable_to_non_nullable
          : photoUrl as String?,
      appointmentCount: appointmentCount == const $CopyWithPlaceholder()
          ? _value.appointmentCount
          // ignore: cast_nullable_to_non_nullable
          : appointmentCount as int,
      hasGapAvailability: hasGapAvailability == const $CopyWithPlaceholder()
          ? _value.hasGapAvailability
          // ignore: cast_nullable_to_non_nullable
          : hasGapAvailability as bool,
    );
  }
}

extension $CalendarViewStaffInnerCopyWith on CalendarViewStaffInner {
  /// Returns a callable class that can be used as follows: `instanceOfCalendarViewStaffInner.copyWith(...)` or like so:`instanceOfCalendarViewStaffInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CalendarViewStaffInnerCWProxy get copyWith =>
      _$CalendarViewStaffInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CalendarViewStaffInner _$CalendarViewStaffInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CalendarViewStaffInner', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'staffId',
      'fullName',
      'calendarColor',
      'appointmentCount',
      'hasGapAvailability',
    ],
  );
  final val = CalendarViewStaffInner(
    staffId: $checkedConvert('staffId', (v) => v as String),
    fullName: $checkedConvert('fullName', (v) => v as String),
    calendarColor: $checkedConvert('calendarColor', (v) => v as String),
    photoUrl: $checkedConvert('photoUrl', (v) => v as String?),
    appointmentCount: $checkedConvert(
      'appointmentCount',
      (v) => (v as num).toInt(),
    ),
    hasGapAvailability: $checkedConvert('hasGapAvailability', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$CalendarViewStaffInnerToJson(
  CalendarViewStaffInner instance,
) => <String, dynamic>{
  'staffId': instance.staffId,
  'fullName': instance.fullName,
  'calendarColor': instance.calendarColor,
  'photoUrl': ?instance.photoUrl,
  'appointmentCount': instance.appointmentCount,
  'hasGapAvailability': instance.hasGapAvailability,
};
