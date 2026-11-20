// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'staff_detail_all_of_working_hours.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$StaffDetailAllOfWorkingHoursCWProxy {
  StaffDetailAllOfWorkingHours branchId(String branchId);

  StaffDetailAllOfWorkingHours hours(WeeklyHours hours);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffDetailAllOfWorkingHours(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffDetailAllOfWorkingHours(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffDetailAllOfWorkingHours call({String branchId, WeeklyHours hours});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfStaffDetailAllOfWorkingHours.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfStaffDetailAllOfWorkingHours.copyWith.fieldName(...)`
class _$StaffDetailAllOfWorkingHoursCWProxyImpl
    implements _$StaffDetailAllOfWorkingHoursCWProxy {
  const _$StaffDetailAllOfWorkingHoursCWProxyImpl(this._value);

  final StaffDetailAllOfWorkingHours _value;

  @override
  StaffDetailAllOfWorkingHours branchId(String branchId) =>
      this(branchId: branchId);

  @override
  StaffDetailAllOfWorkingHours hours(WeeklyHours hours) => this(hours: hours);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffDetailAllOfWorkingHours(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffDetailAllOfWorkingHours(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffDetailAllOfWorkingHours call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? hours = const $CopyWithPlaceholder(),
  }) {
    return StaffDetailAllOfWorkingHours(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      hours: hours == const $CopyWithPlaceholder()
          ? _value.hours
          // ignore: cast_nullable_to_non_nullable
          : hours as WeeklyHours,
    );
  }
}

extension $StaffDetailAllOfWorkingHoursCopyWith
    on StaffDetailAllOfWorkingHours {
  /// Returns a callable class that can be used as follows: `instanceOfStaffDetailAllOfWorkingHours.copyWith(...)` or like so:`instanceOfStaffDetailAllOfWorkingHours.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$StaffDetailAllOfWorkingHoursCWProxy get copyWith =>
      _$StaffDetailAllOfWorkingHoursCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

StaffDetailAllOfWorkingHours _$StaffDetailAllOfWorkingHoursFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('StaffDetailAllOfWorkingHours', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['branchId', 'hours']);
  final val = StaffDetailAllOfWorkingHours(
    branchId: $checkedConvert('branchId', (v) => v as String),
    hours: $checkedConvert(
      'hours',
      (v) => WeeklyHours.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$StaffDetailAllOfWorkingHoursToJson(
  StaffDetailAllOfWorkingHours instance,
) => <String, dynamic>{
  'branchId': instance.branchId,
  'hours': instance.hours.toJson(),
};
