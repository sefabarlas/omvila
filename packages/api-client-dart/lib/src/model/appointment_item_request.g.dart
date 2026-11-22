// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_item_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentItemRequestCWProxy {
  AppointmentItemRequest serviceId(String serviceId);

  AppointmentItemRequest staffId(String? staffId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentItemRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentItemRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentItemRequest call({String serviceId, String? staffId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentItemRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentItemRequest.copyWith.fieldName(...)`
class _$AppointmentItemRequestCWProxyImpl
    implements _$AppointmentItemRequestCWProxy {
  const _$AppointmentItemRequestCWProxyImpl(this._value);

  final AppointmentItemRequest _value;

  @override
  AppointmentItemRequest serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  AppointmentItemRequest staffId(String? staffId) => this(staffId: staffId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentItemRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentItemRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentItemRequest call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
  }) {
    return AppointmentItemRequest(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
    );
  }
}

extension $AppointmentItemRequestCopyWith on AppointmentItemRequest {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentItemRequest.copyWith(...)` or like so:`instanceOfAppointmentItemRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentItemRequestCWProxy get copyWith =>
      _$AppointmentItemRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentItemRequest _$AppointmentItemRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AppointmentItemRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['serviceId']);
  final val = AppointmentItemRequest(
    serviceId: $checkedConvert('serviceId', (v) => v as String),
    staffId: $checkedConvert('staffId', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AppointmentItemRequestToJson(
  AppointmentItemRequest instance,
) => <String, dynamic>{
  'serviceId': instance.serviceId,
  'staffId': instance.staffId,
};
