// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'walk_in_started.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WalkInStartedCWProxy {
  WalkInStarted ticket(WalkInTicketView ticket);

  WalkInStarted appointment(AppointmentDetail appointment);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInStarted(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInStarted(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInStarted call({WalkInTicketView ticket, AppointmentDetail appointment});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWalkInStarted.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWalkInStarted.copyWith.fieldName(...)`
class _$WalkInStartedCWProxyImpl implements _$WalkInStartedCWProxy {
  const _$WalkInStartedCWProxyImpl(this._value);

  final WalkInStarted _value;

  @override
  WalkInStarted ticket(WalkInTicketView ticket) => this(ticket: ticket);

  @override
  WalkInStarted appointment(AppointmentDetail appointment) =>
      this(appointment: appointment);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInStarted(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInStarted(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInStarted call({
    Object? ticket = const $CopyWithPlaceholder(),
    Object? appointment = const $CopyWithPlaceholder(),
  }) {
    return WalkInStarted(
      ticket: ticket == const $CopyWithPlaceholder()
          ? _value.ticket
          // ignore: cast_nullable_to_non_nullable
          : ticket as WalkInTicketView,
      appointment: appointment == const $CopyWithPlaceholder()
          ? _value.appointment
          // ignore: cast_nullable_to_non_nullable
          : appointment as AppointmentDetail,
    );
  }
}

extension $WalkInStartedCopyWith on WalkInStarted {
  /// Returns a callable class that can be used as follows: `instanceOfWalkInStarted.copyWith(...)` or like so:`instanceOfWalkInStarted.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WalkInStartedCWProxy get copyWith => _$WalkInStartedCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WalkInStarted _$WalkInStartedFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WalkInStarted', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['ticket', 'appointment']);
      final val = WalkInStarted(
        ticket: $checkedConvert(
          'ticket',
          (v) => WalkInTicketView.fromJson(v as Map<String, dynamic>),
        ),
        appointment: $checkedConvert(
          'appointment',
          (v) => AppointmentDetail.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$WalkInStartedToJson(WalkInStarted instance) =>
    <String, dynamic>{
      'ticket': instance.ticket.toJson(),
      'appointment': instance.appointment.toJson(),
    };
