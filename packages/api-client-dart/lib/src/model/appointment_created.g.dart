// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_created.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentCreatedCWProxy {
  AppointmentCreated appointment(AppointmentDetail appointment);

  AppointmentCreated skippedOccurrences(List<DateTime> skippedOccurrences);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentCreated(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentCreated(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentCreated call({
    AppointmentDetail appointment,
    List<DateTime> skippedOccurrences,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentCreated.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentCreated.copyWith.fieldName(...)`
class _$AppointmentCreatedCWProxyImpl implements _$AppointmentCreatedCWProxy {
  const _$AppointmentCreatedCWProxyImpl(this._value);

  final AppointmentCreated _value;

  @override
  AppointmentCreated appointment(AppointmentDetail appointment) =>
      this(appointment: appointment);

  @override
  AppointmentCreated skippedOccurrences(List<DateTime> skippedOccurrences) =>
      this(skippedOccurrences: skippedOccurrences);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentCreated(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentCreated(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentCreated call({
    Object? appointment = const $CopyWithPlaceholder(),
    Object? skippedOccurrences = const $CopyWithPlaceholder(),
  }) {
    return AppointmentCreated(
      appointment: appointment == const $CopyWithPlaceholder()
          ? _value.appointment
          // ignore: cast_nullable_to_non_nullable
          : appointment as AppointmentDetail,
      skippedOccurrences: skippedOccurrences == const $CopyWithPlaceholder()
          ? _value.skippedOccurrences
          // ignore: cast_nullable_to_non_nullable
          : skippedOccurrences as List<DateTime>,
    );
  }
}

extension $AppointmentCreatedCopyWith on AppointmentCreated {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentCreated.copyWith(...)` or like so:`instanceOfAppointmentCreated.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentCreatedCWProxy get copyWith =>
      _$AppointmentCreatedCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentCreated _$AppointmentCreatedFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AppointmentCreated', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['appointment', 'skippedOccurrences'],
      );
      final val = AppointmentCreated(
        appointment: $checkedConvert(
          'appointment',
          (v) => AppointmentDetail.fromJson(v as Map<String, dynamic>),
        ),
        skippedOccurrences: $checkedConvert(
          'skippedOccurrences',
          (v) => (v as List<dynamic>)
              .map((e) => const DateOnlyConverter().fromJson(e as String))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AppointmentCreatedToJson(AppointmentCreated instance) =>
    <String, dynamic>{
      'appointment': instance.appointment.toJson(),
      'skippedOccurrences': instance.skippedOccurrences
          .map(const DateOnlyConverter().toJson)
          .toList(),
    };
