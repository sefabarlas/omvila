// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'time_off_created.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TimeOffCreatedCWProxy {
  TimeOffCreated timeOff(TimeOff timeOff);

  TimeOffCreated affectedAppointments(
    List<CalendarAppointment> affectedAppointments,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeOffCreated(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeOffCreated(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeOffCreated call({
    TimeOff timeOff,
    List<CalendarAppointment> affectedAppointments,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTimeOffCreated.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTimeOffCreated.copyWith.fieldName(...)`
class _$TimeOffCreatedCWProxyImpl implements _$TimeOffCreatedCWProxy {
  const _$TimeOffCreatedCWProxyImpl(this._value);

  final TimeOffCreated _value;

  @override
  TimeOffCreated timeOff(TimeOff timeOff) => this(timeOff: timeOff);

  @override
  TimeOffCreated affectedAppointments(
    List<CalendarAppointment> affectedAppointments,
  ) => this(affectedAppointments: affectedAppointments);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeOffCreated(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeOffCreated(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeOffCreated call({
    Object? timeOff = const $CopyWithPlaceholder(),
    Object? affectedAppointments = const $CopyWithPlaceholder(),
  }) {
    return TimeOffCreated(
      timeOff: timeOff == const $CopyWithPlaceholder()
          ? _value.timeOff
          // ignore: cast_nullable_to_non_nullable
          : timeOff as TimeOff,
      affectedAppointments: affectedAppointments == const $CopyWithPlaceholder()
          ? _value.affectedAppointments
          // ignore: cast_nullable_to_non_nullable
          : affectedAppointments as List<CalendarAppointment>,
    );
  }
}

extension $TimeOffCreatedCopyWith on TimeOffCreated {
  /// Returns a callable class that can be used as follows: `instanceOfTimeOffCreated.copyWith(...)` or like so:`instanceOfTimeOffCreated.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TimeOffCreatedCWProxy get copyWith => _$TimeOffCreatedCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TimeOffCreated _$TimeOffCreatedFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TimeOffCreated', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['timeOff', 'affectedAppointments']);
      final val = TimeOffCreated(
        timeOff: $checkedConvert(
          'timeOff',
          (v) => TimeOff.fromJson(v as Map<String, dynamic>),
        ),
        affectedAppointments: $checkedConvert(
          'affectedAppointments',
          (v) => (v as List<dynamic>)
              .map(
                (e) => CalendarAppointment.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$TimeOffCreatedToJson(TimeOffCreated instance) =>
    <String, dynamic>{
      'timeOff': instance.timeOff.toJson(),
      'affectedAppointments': instance.affectedAppointments
          .map((e) => e.toJson())
          .toList(),
    };
