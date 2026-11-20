// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_reschedule_appointment_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicRescheduleAppointmentRequestCWProxy {
  PublicRescheduleAppointmentRequest startUtc(DateTime startUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicRescheduleAppointmentRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicRescheduleAppointmentRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicRescheduleAppointmentRequest call({DateTime startUtc});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicRescheduleAppointmentRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicRescheduleAppointmentRequest.copyWith.fieldName(...)`
class _$PublicRescheduleAppointmentRequestCWProxyImpl
    implements _$PublicRescheduleAppointmentRequestCWProxy {
  const _$PublicRescheduleAppointmentRequestCWProxyImpl(this._value);

  final PublicRescheduleAppointmentRequest _value;

  @override
  PublicRescheduleAppointmentRequest startUtc(DateTime startUtc) =>
      this(startUtc: startUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicRescheduleAppointmentRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicRescheduleAppointmentRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicRescheduleAppointmentRequest call({
    Object? startUtc = const $CopyWithPlaceholder(),
  }) {
    return PublicRescheduleAppointmentRequest(
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
    );
  }
}

extension $PublicRescheduleAppointmentRequestCopyWith
    on PublicRescheduleAppointmentRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPublicRescheduleAppointmentRequest.copyWith(...)` or like so:`instanceOfPublicRescheduleAppointmentRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicRescheduleAppointmentRequestCWProxy get copyWith =>
      _$PublicRescheduleAppointmentRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicRescheduleAppointmentRequest _$PublicRescheduleAppointmentRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicRescheduleAppointmentRequest', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['startUtc']);
  final val = PublicRescheduleAppointmentRequest(
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
  );
  return val;
});

Map<String, dynamic> _$PublicRescheduleAppointmentRequestToJson(
  PublicRescheduleAppointmentRequest instance,
) => <String, dynamic>{'startUtc': instance.startUtc.toIso8601String()};
