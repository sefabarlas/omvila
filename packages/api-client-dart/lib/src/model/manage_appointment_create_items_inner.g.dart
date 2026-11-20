// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_appointment_create_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageAppointmentCreateItemsInnerCWProxy {
  ManageAppointmentCreateItemsInner serviceId(String serviceId);

  ManageAppointmentCreateItemsInner staffId(String? staffId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAppointmentCreateItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAppointmentCreateItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAppointmentCreateItemsInner call({String serviceId, String? staffId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageAppointmentCreateItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageAppointmentCreateItemsInner.copyWith.fieldName(...)`
class _$ManageAppointmentCreateItemsInnerCWProxyImpl
    implements _$ManageAppointmentCreateItemsInnerCWProxy {
  const _$ManageAppointmentCreateItemsInnerCWProxyImpl(this._value);

  final ManageAppointmentCreateItemsInner _value;

  @override
  ManageAppointmentCreateItemsInner serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  ManageAppointmentCreateItemsInner staffId(String? staffId) =>
      this(staffId: staffId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAppointmentCreateItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAppointmentCreateItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAppointmentCreateItemsInner call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
  }) {
    return ManageAppointmentCreateItemsInner(
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

extension $ManageAppointmentCreateItemsInnerCopyWith
    on ManageAppointmentCreateItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfManageAppointmentCreateItemsInner.copyWith(...)` or like so:`instanceOfManageAppointmentCreateItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageAppointmentCreateItemsInnerCWProxy get copyWith =>
      _$ManageAppointmentCreateItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageAppointmentCreateItemsInner _$ManageAppointmentCreateItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageAppointmentCreateItemsInner', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['serviceId', 'staffId']);
  final val = ManageAppointmentCreateItemsInner(
    serviceId: $checkedConvert('serviceId', (v) => v as String),
    staffId: $checkedConvert('staffId', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$ManageAppointmentCreateItemsInnerToJson(
  ManageAppointmentCreateItemsInner instance,
) => <String, dynamic>{
  'serviceId': instance.serviceId,
  'staffId': instance.staffId,
};
