// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_create_use_package_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentCreateUsePackageItemsInnerCWProxy {
  PublicAppointmentCreateUsePackageItemsInner serviceId(String serviceId);

  PublicAppointmentCreateUsePackageItemsInner customerPackageItemId(
    String customerPackageItemId,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentCreateUsePackageItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentCreateUsePackageItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentCreateUsePackageItemsInner call({
    String serviceId,
    String customerPackageItemId,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentCreateUsePackageItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentCreateUsePackageItemsInner.copyWith.fieldName(...)`
class _$PublicAppointmentCreateUsePackageItemsInnerCWProxyImpl
    implements _$PublicAppointmentCreateUsePackageItemsInnerCWProxy {
  const _$PublicAppointmentCreateUsePackageItemsInnerCWProxyImpl(this._value);

  final PublicAppointmentCreateUsePackageItemsInner _value;

  @override
  PublicAppointmentCreateUsePackageItemsInner serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  PublicAppointmentCreateUsePackageItemsInner customerPackageItemId(
    String customerPackageItemId,
  ) => this(customerPackageItemId: customerPackageItemId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentCreateUsePackageItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentCreateUsePackageItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentCreateUsePackageItemsInner call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? customerPackageItemId = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentCreateUsePackageItemsInner(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      customerPackageItemId:
          customerPackageItemId == const $CopyWithPlaceholder()
          ? _value.customerPackageItemId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageItemId as String,
    );
  }
}

extension $PublicAppointmentCreateUsePackageItemsInnerCopyWith
    on PublicAppointmentCreateUsePackageItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentCreateUsePackageItemsInner.copyWith(...)` or like so:`instanceOfPublicAppointmentCreateUsePackageItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentCreateUsePackageItemsInnerCWProxy get copyWith =>
      _$PublicAppointmentCreateUsePackageItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentCreateUsePackageItemsInner
_$PublicAppointmentCreateUsePackageItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentCreateUsePackageItemsInner', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['serviceId', 'customerPackageItemId']);
  final val = PublicAppointmentCreateUsePackageItemsInner(
    serviceId: $checkedConvert('serviceId', (v) => v as String),
    customerPackageItemId: $checkedConvert(
      'customerPackageItemId',
      (v) => v as String,
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicAppointmentCreateUsePackageItemsInnerToJson(
  PublicAppointmentCreateUsePackageItemsInner instance,
) => <String, dynamic>{
  'serviceId': instance.serviceId,
  'customerPackageItemId': instance.customerPackageItemId,
};
